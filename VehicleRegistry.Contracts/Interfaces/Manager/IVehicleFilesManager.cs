using VehicleRegistry.Contracts.Manager.VehicleFiles;

namespace VehicleRegistry.Contracts.Interfaces.Manager
{
    public interface IVehicleFilesManager
    {
        /// <summary>
        /// Retrieves all files associated with the specified vehicle.
        /// </summary>
        /// <param name="vehicleId">The unique identifier of the vehicle.</param>
        /// <returns>A list of files related to the vehicle.</returns>
        Task<List<VehicleFileModel>> GetVehicleFilesAsync(int vehicleId);

        /// <summary>
        /// Saves metadata for a new vehicle file to the database or storage system.
        /// </summary>
        /// <param name="vehicleId">The unique identifier of the vehicle associated with the file.</param>
        /// <param name="fileName">The name of the file to be saved.</param>
        /// <param name="mimeType">The MIME type of the file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveVehicleFileDataAsync(int vehicleId, string fileName, string mimeType);

        /// <summary>
        /// Marks a file as processed using its object key and the associated event timestamp.
        /// </summary>
        /// <param name="objectKey">The unique key identifying the file in the storage.</param>
        /// <param name="eventTime">The timestamp when the file was processed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkFileAsProcessedAsync(string objectKey, DateTime eventTime);
    }
}