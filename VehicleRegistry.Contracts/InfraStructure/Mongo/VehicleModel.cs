using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace VehicleRegistry.Contracts.InfraStructure.Mongo
{
    [BsonIgnoreExtraElements]
    public class VehicleModel
    {
        [BsonId]
        [Required]
        [BsonElement("plate")]
        public string Plate { get; set; } = string.Empty;

        [Required]
        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [Required]
        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("brand")]
        public string Brand { get; set; } = string.Empty;
    }
}
