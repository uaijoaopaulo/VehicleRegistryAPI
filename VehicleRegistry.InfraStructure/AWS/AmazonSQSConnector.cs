using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using VehicleRegistry.Contracts.InfraStructure.AWS;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Aws;

namespace VehicleRegistry.InfraStructure.AWS
{
    public class AmazonSQSConnector(IAmazonSQS amazonSQSClient) : IAmazonSQSConnector
    {
        private readonly IAmazonSQS _amazonSQSClient = amazonSQSClient;

        public async Task SendMessageToQueueAsync<T>(T data, string queueUrl)
        {
            if (string.IsNullOrEmpty(queueUrl))
            {
                throw new ArgumentNullException(nameof(queueUrl));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var messageBody = JsonConvert.SerializeObject(data);

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            await _amazonSQSClient.SendMessageAsync(sendMessageRequest);
        }

        public async Task<List<SQSMessage<T>>> ReceiveMessagesAsync<T>(string queueUrl, int visibilityTimeout)
        {
            if (string.IsNullOrEmpty(queueUrl))
            {
                throw new ArgumentNullException(nameof(queueUrl));
            }

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                VisibilityTimeout = visibilityTimeout,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            };

            var receiveMessageResponse = await _amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);

            if (receiveMessageResponse.Messages == null || receiveMessageResponse.Messages.Count == 0)
            {
                return [];
            }

            var messages = receiveMessageResponse.Messages.Select(message => new SQSMessage<T>
            {
                MessageId = message.MessageId,
                ReceiptHandle = message.ReceiptHandle,
                Body = JsonConvert.DeserializeObject<T>(message.Body)
            }).ToList();

            return messages;
        }

        public async Task DeleteMessagesInBatchAsync<T>(List<SQSMessage<T>> messages, string queueUrl)
        {
            if (string.IsNullOrEmpty(queueUrl))
            {
                throw new ArgumentNullException(nameof(queueUrl));
            }

            if (messages == null || messages.Count == 0)
            {
                return;
            }

            var deleteMessageBatchRequest = new DeleteMessageBatchRequest
            {
                QueueUrl = queueUrl,
                Entries = messages.Select(message => new DeleteMessageBatchRequestEntry
                {
                    Id = message.MessageId,
                    ReceiptHandle = message.ReceiptHandle
                }).ToList()
            };

            await _amazonSQSClient.DeleteMessageBatchAsync(deleteMessageBatchRequest);
        }

        public async Task DeleteMessageAsync<T>(SQSMessage<T> message, string queueUrl)
        {
            if (string.IsNullOrEmpty(queueUrl))
            {
                throw new ArgumentNullException(nameof(queueUrl));
            }

            await _amazonSQSClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
        }
    }
}
