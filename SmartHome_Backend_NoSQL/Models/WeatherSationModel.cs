using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class WeatherSationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("Temp")]
        [JsonPropertyName("temp")]
        public double? temp { get; set; } 

        [BsonElement("TempMin")]
        [JsonPropertyName("tempMin")]
        public double? tempMin { get; set; } 

        [BsonElement("TempMax")]
        [JsonPropertyName("tempMax")]
        public double? tempMax { get; set; } 

        [BsonElement("Wind")]
        [JsonPropertyName("wind")]
        public double? wind { get; set; } 

        [BsonElement("WindMin")]
        [JsonPropertyName("windMin")]
        public double? windMin { get; set; } 

        [BsonElement("WindMax")]
        [JsonPropertyName("windMax")]
        public double? windMax { get; set; }

        [BsonElement("HumidityMax")]
        [JsonPropertyName("humidityMax")]
        public double? humidityMax { get; set; }

        [BsonElement("HumidityMin")]
        [JsonPropertyName("humidityMin")]
        public double? humidityMin { get; set; }

        [BsonElement("Humidity")]
        [JsonPropertyName("humidity")]
        public double? humidity { get; set; }

        [BsonElement("Rain")]
        [JsonPropertyName("rain")]
        public double? rain { get; set; }

        [BsonElement("Raining")]
        [JsonPropertyName("raining")]
        public bool? raining { get; set; }

        [BsonElement("SunDuration")]
        [JsonPropertyName("sunDuration")]
        public double? sunDuration { get; set; }

        [BsonElement("DayTime")]
        [JsonPropertyName("dayTime")]
        public string? daytime { get; set; }
    }
}
