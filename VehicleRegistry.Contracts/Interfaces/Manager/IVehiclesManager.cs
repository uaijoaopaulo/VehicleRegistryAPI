using VehicleRegistry.Contracts.InfraStructure.Mongo;

namespace VehicleRegistry.Contracts.Interfaces.Manager
{
    public interface IVehiclesManager
    {
        Task<List<VehicleModel>> GetVehiclesAsync(string? plate, List<string>? plates, int? page = null, int? pageSize = null);
        Task<VehicleModel> InsertModelAsync(VehicleModel vehicleModel);
        Task<VehicleModel> UpdateVehicleAsync(string plate, VehicleModel vehicleModel);
        Task DeleteVehicleAsync(string plate);
    }
}