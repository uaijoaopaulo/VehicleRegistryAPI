using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace VehicleRegistry.Contracts.InfraStructure.Mongo
{

    [BsonIgnoreExtraElements]
    public class UserModel
    {
        [BsonId]
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("roles")]
        public List<string> Roles { get; set; } = [];
    }
}
