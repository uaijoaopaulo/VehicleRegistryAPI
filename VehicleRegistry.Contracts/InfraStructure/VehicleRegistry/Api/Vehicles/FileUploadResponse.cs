namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Vehicles
{
    public class FileUploadResponse
    {
        public string FileName { get; set; }
        public string FileMimetype { get; set; }
        public string UploadUrl { get; set; }
    }
}