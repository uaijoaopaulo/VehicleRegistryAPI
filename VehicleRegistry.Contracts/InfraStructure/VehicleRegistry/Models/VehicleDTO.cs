using System.ComponentModel.DataAnnotations;

namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Models
{
    public class VehicleDTO
    {
        public int Id { get; set; } 
        public string? Make { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = "Year is required.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Plate is required.")]
        public string Plate { get; set; } = null!;
    }
}
