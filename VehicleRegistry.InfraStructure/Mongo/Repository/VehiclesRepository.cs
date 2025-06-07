using MongoDB.Driver;
using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Mongo;

namespace VehicleRegistry.InfraStructure.Mongo.Repository
{
    public class VehiclesRepository(IMongoDatabase mongoDatabase) : MongoBaseClient<VehicleModel>(mongoDatabase), IVehiclesRepository
    {
        protected override string GetCollectionName<T>()
        {
            return "vehicles";
        }

        public async Task<List<VehicleModel>> GetVehiclesAsync(string? plate, List<string>? plates, int? page = null, int? pageSize = null)
        {
            var filterBuilder = Builders<VehicleModel>.Filter;
            var filters = new List<FilterDefinition<VehicleModel>>();

            if (!string.IsNullOrWhiteSpace(plate))
            {
                filters.Add(filterBuilder.Eq(v => v.Plate, plate));
            }

            if (plates is { Count: > 0 })
            {
                filters.Add(filterBuilder.In(v => v.Plate, plates));
            }

            var filter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            var sort = Builders<VehicleModel>.Sort.Ascending(v => v.Brand).Ascending(v => v.Model);

            int? skip = null;
            int? limit = null;

            if (page.HasValue && pageSize.HasValue)
            {
                skip = (page.Value - 1) * pageSize.Value;
                limit = pageSize;
            }

            return await GetAllAsync(filter, sort, skip, limit);
        }

        public async Task<VehicleModel> UpdateVehicleAsync(VehicleModel vehicle)
        {
            var filter = Builders<VehicleModel>.Filter.Eq(x => x.Plate, vehicle.Plate);
            return await ReplaceOneAsync(filter, vehicle);
        }

        public async Task DeleteVehicleAsync(string plate)
        {
            var filter = Builders<VehicleModel>.Filter.Eq(x => x.Plate, plate);
            await DeleteOneAsync(filter);
        }
    }
}
