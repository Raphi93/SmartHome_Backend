using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class IndoorTempAverageModel
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

        [BsonElement("DayTime")]
        [JsonPropertyName("dayTime")]
        public string? daytime { get; set; }

    }
}
