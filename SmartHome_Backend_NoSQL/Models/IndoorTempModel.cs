using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class IndoorTempModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("WallTemp")]
        [JsonPropertyName("wallTemp")]
        public double? wallTemp { get; set; }

        [BsonElement("FloorTemp")]
        [JsonPropertyName("floorTemp")]
        public double? floorTemp { get; set; }

        [BsonElement("WallTempMax")]
        [JsonPropertyName("wallTempMax")]
        public double? wallTempMax { get; set; }

        [BsonElement("FloorTempMax")]
        [JsonPropertyName("floorTempMax")]
        public double? floorTempMax { get; set; }

        [BsonElement("WallTempMin")]
        [JsonPropertyName("wallTempMin")]
        public double? wallTempMin { get; set; }

        [BsonElement("FloorTemp")]
        [JsonPropertyName("floorTemp")]
        public double? floorTempMin { get; set; }

        [BsonElement("DayTime")]
        [JsonPropertyName("dayTime")]
        public string? daytime { get; set; }

        [BsonElement("ID")]
        [JsonPropertyName("id")]
        public int? id { get; set; }
    }
}
