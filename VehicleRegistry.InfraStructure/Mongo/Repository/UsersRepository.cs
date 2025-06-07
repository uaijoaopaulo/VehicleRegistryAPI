using MongoDB.Driver;
using VehicleRegistry.Contracts.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Interfaces.Mongo;

namespace VehicleRegistry.InfraStructure.Mongo.Repository
{
    public class UsersRepository(IMongoDatabase mongoDatabase) : MongoBaseClient<UserModel>(mongoDatabase), IUsersRepository
    {
        protected override string GetCollectionName<T>()
        {
            return "users";
        }

        public async Task<UserModel?> GetUserByAuthAsync(string email, string password)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Email, email)
                  & Builders<UserModel>.Filter.Eq(x => x.Password, password);
            return await GetOneAsync(filter);
        }
    }
}