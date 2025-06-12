namespace VehicleRegistry.Contracts.InfraStructure.AWS.AWSConfig
{
    public class AwsOptions
    {
        public string Profile { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public List<AwsQueueSettings> AWSQueues { get; set; } = new();
    }
}
