using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class SaveUP
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("_id")]
        public string? ID { get; set; }

        [BsonElement("Produkt")]
        [JsonPropertyName("produkt")]
        public string? Produkt { get; set; }

        [BsonElement("Kategorie")]
        [JsonPropertyName("kategorie")]
        public string? Kategorie { get; set; }

        [BsonElement("Wert")]
        [JsonPropertyName("wert")]
        public double? Wert { get; set; }

        [BsonElement("Datum")]
        [JsonPropertyName("datum")]
        public DateTime Datum { get; set; }

        [BsonElement("Name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
