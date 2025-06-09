using VehicleRegistry.Contracts.Manager.VehicleFiles;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo
{
    public interface IVehicleFilesRepository : IRepositoryMongoBase<VehicleFileModel>
    {
        /// <summary>
        /// Retrieves all files associated with the specified vehicle.
        /// </summary>
        /// <param name="vehicleId">The unique identifier of the vehicle.</param>
        /// <returns>A list of files related to the vehicle.</returns>
        Task<List<VehicleFileModel>> GetVehicleFileAsync(int vehicleId);

        /// <summary>
        /// Marks a file as processed using its object key and the corresponding event timestamp.
        /// </summary>
        /// <param name="objectKey">The unique key of the file in the storage.</param>
        /// <param name="eventTime">The timestamp of the processing event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkFileAsProcessedAsync(string objectKey, DateTime eventTime);
    }
}