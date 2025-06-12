namespace VehicleRegistry.Contracts.InfraStructure.AWS.AWSConfig
{
    public class AwsQueueSettings
    {
        public string Id { get; set; }
        public string QueueName { get; set; } = string.Empty;
        public string? QueueUrl { get; set; }
        public string? QueueArn { get; set; }
        public string? BucketName { get; set; }
    }
}