using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class WeatherAverageModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("Temp")]
        [JsonPropertyName("temp")]
        public double? temp { get; set; }

        [BsonElement("Wind")]
        [JsonPropertyName("wind")]
        public double? wind { get; set; }

        [BsonElement("Humidity")]
        [JsonPropertyName("humidity")]
        public double? humidity { get; set; }
    }
}
