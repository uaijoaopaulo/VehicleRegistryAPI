using VehicleRegistry.Contracts.InfraStructure.Mongo;

namespace VehicleRegistry.Contracts.Interfaces.Mongo
{
    public interface IVehiclesRepository : IRepositoryMongoBase<VehicleModel>
    {
        /// <summary>
        /// Asynchronously retrieves a list of documents from the MongoDB collection based on the specified filter,
        /// with optional sorting, pagination (skip), and limiting of results.
        /// </summary>
        /// <param name="filter">The filter definition to apply to the query.</param>
        /// <param name="sort">An optional sort definition to order the results.</param>
        /// <param name="skip">An optional number of documents to skip (used for pagination).</param>
        /// <param name="limit">An optional limit on the number of documents to return.</param>
        /// <returns>A task representing the asynchronous operation, containing the list of matching documents.</returns>
        Task<List<VehicleModel>> GetVehiclesAsync(string? plate, List<string>? plates, int? page = null, int? pageSize = null);

        /// <summary>
        /// Asynchronously updates an existing vehicle document in the MongoDB collection.
        /// </summary>
        /// <param name="vehicle">The vehicle model containing updated information.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated document.</returns>
        Task<VehicleModel> UpdateVehicleAsync(VehicleModel vehicle);

        /// <summary>
        /// Asynchronously deletes a vehicle document from the MongoDB collection by its license plate.
        /// </summary>
        /// <param name="plate">The license plate of the vehicle to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task DeleteVehicleAsync(string plate);
    }
}