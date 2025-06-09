using VehicleRegistry.Contracts.Manager.Vehicle;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Database
{
    public interface IVehiclesRepository : IDbRepository<VehicleDTO>
    {
        /// <summary>
        /// Retrieves a list of vehicles based on optional filtering criteria and pagination parameters.
        /// </summary>
        /// <param name="plate">Optional license plate to filter the vehicles.</param>
        /// <param name="ids">Optional list of vehicle IDs to filter the results.</param>
        /// <param name="page">Optional page number for pagination (starting from 1).</param>
        /// <param name="pageSize">Optional number of items per page.</param>
        /// <returns>A list of vehicles matching the specified criteria.</returns>
        Task<List<VehicleDTO>> GetVehiclesAsync(string? plate, List<int>? ids, int? page = null, int? pageSize = null);
    }
}