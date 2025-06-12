using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using VehicleRegistry.Contracts.InfraStructure.AWS.AWSConfig;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;

namespace VehicleRegistry.InfraStructure.AWS
{
    public class AmazonConnector(IOptions<AwsOptions> awsOptions, IAmazonS3 amazonS3Client, IAmazonSQS amazonSQSClient) : IAmazonConnector
    {
        private readonly IAmazonS3 _amazonS3Client = amazonS3Client;
        private readonly IAmazonSQS _amazonSQSClient = amazonSQSClient;
        private readonly AwsOptions _awsOptions = awsOptions.Value;

        public AwsQueueSettings? GetQueueById(string queueName)
        {
            return _awsOptions.AWSQueues.FirstOrDefault(q => q.Id == queueName);
        }

        public async Task EnsureBucketAsync()
        {
            var bucketList = await _amazonS3Client.ListBucketsAsync();
            foreach (var bucketName in _awsOptions.AWSQueues.Select(queue => queue.BucketName))
            {
                if (!bucketList.Buckets.Any(b => b.BucketName == bucketName))
                {
                    await _amazonS3Client.PutBucketAsync(new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    });
                }
            }
        }

        public async Task EnsureQueuesAsync()
        {
            foreach (var awsQueue in _awsOptions.AWSQueues)
            {
                string queueUrl;
                bool created = false;

                try
                {
                    var resp = await _amazonSQSClient.GetQueueUrlAsync(awsQueue.QueueName);
                    queueUrl = resp.QueueUrl;
                }
                catch (QueueDoesNotExistException)
                {
                    var resp = await _amazonSQSClient.CreateQueueAsync(awsQueue.QueueName);
                    queueUrl = resp.QueueUrl;
                    created = true;
                }

                var attr = await _amazonSQSClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
                {
                    QueueUrl = queueUrl,
                    AttributeNames = new List<string> { "QueueArn" }
                });

                var queueArn = attr.Attributes["QueueArn"];

                if (!string.IsNullOrEmpty(awsQueue.BucketName) && created)
                {
                    await SetQueuePolicyAsync(queueUrl, queueArn, awsQueue.BucketName);
                    await ConfigureS3NotificationAsync(queueArn, awsQueue.BucketName);
                }

                awsQueue.QueueUrl = queueUrl;
                awsQueue.QueueArn = queueArn;
            }
        }

        private async Task SetQueuePolicyAsync(string queueUrl, string queueArn, string bucketName)
        {
            var policy = @$"{{
              ""Version"": ""2012-10-17"",
              ""Statement"": [
                {{
                  ""Sid"": ""AllowS3SendMessage"",
                  ""Effect"": ""Allow"",
                  ""Principal"": {{ ""Service"": ""s3.amazonaws.com"" }},
                  ""Action"": ""SQS:SendMessage"",
                  ""Resource"": ""{queueArn}"",
                  ""Condition"": {{
                    ""ArnLike"": {{
                      ""aws:SourceArn"": ""arn:aws:s3:::{bucketName}""
                    }}
                  }}
                }}
              ]
            }}";

            await _amazonSQSClient.SetQueueAttributesAsync(new SetQueueAttributesRequest
            {
                QueueUrl = queueUrl,
                Attributes = new Dictionary<string, string>
                {
                    { "Policy", policy }
                }
            });
        }

        private async Task ConfigureS3NotificationAsync(string queueArn, string bucketName)
        {
            var queueConfiguration = new QueueConfiguration
            {
                Queue = queueArn,
                Events = ["s3:ObjectCreated:Put"],
                Id = "S3ToSQSNotification"
            };

            var request = new PutBucketNotificationRequest
            {
                BucketName = bucketName,
                QueueConfigurations = [queueConfiguration]
            };

            await _amazonS3Client.PutBucketNotificationAsync(request);
        }
    }
}
