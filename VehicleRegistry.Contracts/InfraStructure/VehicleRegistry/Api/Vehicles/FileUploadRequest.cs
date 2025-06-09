using System.ComponentModel.DataAnnotations;

namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api.Vehicles
{
    public class FileUploadRequest
    {
        [Required]
        public required string FileName { get; set; }
        [Required]
        public required string FileMimetype { get; set; }
    }
}