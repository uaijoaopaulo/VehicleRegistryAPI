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
        /// Registers metadata for a new vehicle file and generates a pre-signed URL 
        /// to upload the file to the storage service.
        /// </summary>
        /// <param name="vehicleId">The unique identifier of the vehicle associated with the file.</param>
        /// <param name="fileName">The name (including extension) of the file to be uploaded.</param>
        /// <param name="mimeType">The MIME type of the file (e.g., "image/jpeg", "application/pdf").</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing the pre-signed upload URL 
        /// for the specified file.
        /// </returns>
        Task<string> RegisterVehicleFileAndGetUploadUrlAsync(int vehicleId, string fileName, string mimeType);

        /// <summary>
        /// Marks a file as processed using its object key and the associated event timestamp.
        /// </summary>
        /// <param name="objectKey">The unique key identifying the file in the storage.</param>
        /// <param name="eventTime">The timestamp when the file was processed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkFileAsProcessedAsync(string objectKey, DateTime eventTime);
    }
}