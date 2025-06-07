namespace VehicleRegistry.Contracts.InfraStructure.AWS.SQS
{
    public class S3Model
    {
        public BucketModel Bucket { get; set; }
        public ObjectModel Object { get; set; }
    }
}
