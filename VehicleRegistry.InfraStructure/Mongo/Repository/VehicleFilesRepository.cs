﻿using MongoDB.Driver;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Manager.VehicleFiles;

namespace VehicleRegistry.InfraStructure.Mongo.Repository
{
    public class VehicleFilesRepository(IMongoDatabase mongoDatabase) : MongoBaseClient<VehicleFileModel>(mongoDatabase), IVehicleFilesRepository
    {
        protected override string GetCollectionName()
        {
            return "vehicle-files";
        }

        public async Task<List<VehicleFileModel>> GetVehicleFileAsync(int vehicleId)
        {
            var filter = Builders<VehicleFileModel>.Filter.Eq(x => x.VehicleId, vehicleId)
            & Builders<VehicleFileModel>.Filter.Eq(x => x.Status, FileStatus.Uploaded);
            return await GetAllAsync(filter);
        }

        public async Task MarkFileAsProcessedAsync(string objectKey, DateTime eventTime)
        {
            var filter = Builders<VehicleFileModel>.Filter.Eq(x => x.ObjectKey, objectKey);
            var update = Builders<VehicleFileModel>.Update
                .Set(x => x.Status, FileStatus.Uploaded)
                .Set(x => x.GeneratedAt, null)
                .Set(x => x.CreatedAt, eventTime);

            await UpdateOneAsync(filter, update);
        }
    }
}
