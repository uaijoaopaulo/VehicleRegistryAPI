namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api
{
    public class ApiResponse<T>
    {
        public List<string> Errors { get; set; } = [];
        public T? Result { get; set; }
    }
}