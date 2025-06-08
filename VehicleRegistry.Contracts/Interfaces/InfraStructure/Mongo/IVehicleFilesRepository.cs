using VehicleRegistry.Contracts.InfraStructure.Mongo;

namespace VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo
{
    public interface IVehicleFilesRepository : IRepositoryMongoBase<VehicleFileModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehiclePlate"></param>
        /// <returns></returns>
        Task<List<VehicleFileModel>> GetVehicleFileAsync(int idVehicle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="eventTime"></param>
        /// <returns></returns>
        Task MarkFileAsProcessedAsync(string objectKey, DateTime eventTime);
    }
}