using MongoDB.Driver;
using VehicleRegistry.Contracts.Interfaces.InfraStructure.Mongo;
using VehicleRegistry.Contracts.Manager.User;

namespace VehicleRegistry.InfraStructure.Mongo.Repository
{
    public class UsersRepository(IMongoDatabase mongoDatabase) : MongoBaseClient<UserModel>(mongoDatabase), IUsersRepository
    {
        protected override string GetCollectionName()
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