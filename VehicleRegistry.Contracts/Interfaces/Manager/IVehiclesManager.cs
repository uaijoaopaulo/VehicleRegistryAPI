using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Models;

namespace VehicleRegistry.Contracts.Interfaces.Manager
{
    public interface IVehiclesManager
    {
        Task<List<VehicleDTO>> GetVehiclesAsync(string? plate, List<int>? ids, int? page = null, int? pageSize = null);
        Task<VehicleDTO> InsertModelAsync(VehicleDTO vehicleModel);
        Task<VehicleDTO> UpdateVehicleAsync(VehicleDTO vehicleModel);
        Task DeleteVehicleAsync(int id);
    }
}