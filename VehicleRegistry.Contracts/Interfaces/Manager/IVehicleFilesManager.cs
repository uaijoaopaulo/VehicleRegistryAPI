using VehicleRegistry.Contracts.InfraStructure.Mongo;

namespace VehicleRegistry.Contracts.Interfaces.Manager
{
    public interface IVehicleFilesManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="idVehicle"></param>
        /// <returns></returns>
        Task<List<VehicleFileModel>> GetVehicleFilesAsync(int idVehicle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idVehicle"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        Task SaveVehicleFileDataAsync(int idVehicle, string fileName, string contentType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="eventTime"></param>
        /// <returns></returns>
        Task MakeFileAsProcessedAsync(string objectKey, DateTime eventTime);
    }
}