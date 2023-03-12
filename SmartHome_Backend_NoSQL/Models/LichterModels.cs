using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace SmartHome_Backend_NoSQL.Models
{
    public class LichterModels
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("OnOff")]
        [JsonPropertyName("onOff")]
        public bool? OnOff { get; set; }

        [BsonElement("Red")]
        [JsonPropertyName("red")]
        public int? Red { get; set; } = 0;

        [BsonElement("Blue")]
        [JsonPropertyName("blue")]
        public int? Blue { get; set; } = 0;
        
        [BsonElement("Green")]
        [JsonPropertyName("green")]
        public int? Green { get; set; } = 0;

        [BsonElement("Brightness")]
        [JsonPropertyName("brightness")]
        public int? Brightness { get; set; } = 0;

        [BsonElement("Name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
