namespace VehicleRegistry.Contracts.InfraStructure.AWS.SQS
{
    public class RecordModel
    {
        public DateTime? EventTime { get; set; }
        public required S3Model S3 { get; set; }
    }
}