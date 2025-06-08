using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace VehicleRegistry.Contracts.InfraStructure.Mongo
{
    [BsonIgnoreExtraElements]
    public class VehicleFileModel
    {
        [BsonElement("vehicleId")]
        public int VehicleId { get; set; }

        [BsonElement("fileName")]
        public string FileName { get; set; }

        [BsonElement("fileMimetype")]
        public string FileMimetype { get; set; }

        [BsonIgnore]
        public string FileUrl { get; set; }

        [JsonIgnore]
        [BsonElement("objectKey")]
        public string ObjectKey { get; set; }

        [JsonIgnore]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("status")]
        public FileStatus Status { get; set; }

        [BsonElement("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonIgnore]
        [BsonElement("generatedAt")]
        public DateTime? GeneratedAt { get; set; }
    }
}
