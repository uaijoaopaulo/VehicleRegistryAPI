using VehicleRegistry.Contracts.Manager.Vehicle;

namespace VehicleRegistry.Contracts.Interfaces.Manager
{
    public interface IVehiclesManager
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

        /// <summary>
        /// Inserts a new vehicle into the database.
        /// </summary>
        /// <param name="vehicleModel">The vehicle data to insert.</param>
        /// <returns>The inserted vehicle, including any generated fields (e.g., ID).</returns>
        Task<VehicleDTO> InsertModelAsync(VehicleDTO vehicleModel);

        /// <summary>
        /// Updates the information of an existing vehicle in the database.
        /// </summary>
        /// <param name="vehicleModel">The vehicle data with updated information.</param>
        /// <returns>The updated vehicle.</returns>
        Task<VehicleDTO> UpdateVehicleAsync(VehicleDTO vehicleModel);

        /// <summary>
        /// Deletes a vehicle from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the vehicle to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteVehicleAsync(int id);
    }
}