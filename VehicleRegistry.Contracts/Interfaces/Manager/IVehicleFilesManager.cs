using VehicleRegistry.Contracts.InfraStructure.Mongo;

namespace VehicleRegistry.Contracts.Interfaces.Manager
{
    public interface IVehicleFilesManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plate"></param>
        /// <returns></returns>
        Task<List<VehicleFileModel>> GetVehicleFilesAsync(string plate);

        /// <summary>
        /// Asynchronously generates a pre-signed URL for uploading a file associated with a specific vehicle.
        /// </summary>
        /// <param name="vehiclePlate">The license plate of the vehicle the file is associated with.</param>
        /// <param name="fileName">The name of the file to be uploaded.</param>
        /// <param name="contentType">The MIME type of the file.</param>
        /// <returns>A task representing the asynchronous operation, containing the pre-signed upload URL.</returns>
        Task<string> GeneratePresignedUrl(string vehiclePlate, string fileName, string contentType);
    }
}