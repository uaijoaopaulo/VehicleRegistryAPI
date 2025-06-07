using System.Diagnostics.CodeAnalysis;

namespace VehicleRegistry.Contracts.InfraStructure.AWS
{
    [ExcludeFromCodeCoverage]
    public class SQSMessage<T>
    {
        public required string MessageId { get; set; }
        public required string ReceiptHandle { get; set; }
        public T? Body { get; set; }
    }
}
