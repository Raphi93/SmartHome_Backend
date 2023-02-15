namespace SmartHome_Backend_NoSQL.Models
{
    public class WeatherStationDataBaseSetting
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ServicesCollectionName { get; set; } = null!;
    }
}
