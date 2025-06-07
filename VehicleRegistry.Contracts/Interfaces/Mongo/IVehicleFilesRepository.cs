using VehicleRegistry.Contracts.InfraStructure.Mongo;

namespace VehicleRegistry.Contracts.Interfaces.Mongo
{
    public interface IVehicleFilesRepository : IRepositoryMongoBase<VehicleFileModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehiclePlate"></param>
        /// <returns></returns>
        Task<List<VehicleFileModel>> GetVehicleFileAsync(string vehiclePlate);

        /// <summary>
        /// Asynchronously updates the status of the file identified by the specified object key to "Processed",
        /// and records the current UTC timestamp.
        /// </summary>
        /// <param name="objectKey">The unique key identifying the file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkFileAsProcessedAsync(string objectKey);
    }
}