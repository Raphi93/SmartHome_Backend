using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("Name")]
        [JsonPropertyName("name")]
        public string name { get; set; }

        [BsonElement("Passwort")]
        [JsonPropertyName("passwort")]
        public string Passwort { get; set; }

        [BsonElement("ApiKey")]
        [JsonPropertyName("apiKey")]
        public string? ApiKey { get; set; }
    }
}
