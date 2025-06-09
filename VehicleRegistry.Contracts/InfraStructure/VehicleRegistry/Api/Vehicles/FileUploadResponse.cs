namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Vehicles
{
    public class FileUploadResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string FileMimetype { get; set; } = string.Empty;
        public string UploadUrl { get; set; } = string.Empty;
    }
}