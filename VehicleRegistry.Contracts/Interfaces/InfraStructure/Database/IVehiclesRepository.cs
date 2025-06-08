using VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Models;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Database
{
    public interface IVehiclesRepository : IDbRepository<VehicleDTO>
    {
        Task<List<VehicleDTO>> GetVehiclesAsync(string? plate, List<int>? ids, int? page = null, int? pageSize = null);
    }
}