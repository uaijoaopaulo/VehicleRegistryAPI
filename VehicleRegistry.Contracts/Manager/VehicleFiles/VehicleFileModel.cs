using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace VehicleRegistry.Contracts.Manager.VehicleFiles
{
    [BsonIgnoreExtraElements]
    public class VehicleFileModel
    {
        [BsonIgnore]
        public int Id { get; set; }

        [BsonElement("vehicleId")]
        public int VehicleId { get; set; }

        [BsonElement("fileName")]
        public string FileName { get; set; } = string.Empty;

        [BsonElement("fileMimetype")]
        public string FileMimetype { get; set; } = string.Empty;

        [BsonIgnore]
        public string FileUrl { get; set; } = string.Empty;

        [JsonIgnore]
        [BsonElement("objectKey")]
        public string ObjectKey { get; set; } = string.Empty;

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
