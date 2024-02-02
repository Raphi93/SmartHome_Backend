using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; } = string.Empty;

        [BsonElement("User")]
        [JsonPropertyName("user")]
        public string? User { get; set; } = string.Empty;

        [BsonElement("Passwort")]
        [JsonPropertyName("passwort")]
        public string? Passwort { get; set; } = string.Empty;


        [BsonElement("ApiKey")]
        [JsonPropertyName("apiKey")]
        public string? ApiKey { get; set; } = string.Empty; 

        [BsonElement("Role")]
        [JsonPropertyName("role")]
        public string[]? Role { get; set; } = new string[] { };

        [BsonElement("Email")]
        [JsonPropertyName("email")]
        public string? Email { get; set; } = string.Empty;

        [BsonElement("Vorname")]
        [JsonPropertyName("vorname")]
        public string? Vorname { get; set; } = string.Empty;

        [BsonElement("Nachname")]
        [JsonPropertyName("nachname")]
        public string? Nachname { get; set; } = string.Empty;

    }
}
