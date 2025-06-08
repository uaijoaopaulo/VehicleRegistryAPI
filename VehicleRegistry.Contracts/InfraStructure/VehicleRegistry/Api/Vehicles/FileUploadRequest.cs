using System.ComponentModel.DataAnnotations;

namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Vehicles
{
    public class FileUploadRequest
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FileMimetype { get; set; }
    }
}