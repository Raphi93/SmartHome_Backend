namespace SmartHome_Backend_NoSQL.Models
{
    public class SmartHomeDataBaseSetting
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ServicesCollectionName { get; set; } = null!;

        public string AverageCollectionName { get; set; } = null!;

        public string TempCollectionName { get; set; } = null!;

        public string TempAverageCollectionName { get; set; } = null!;

        public string UserCollectionName { get; set; } = null!;
        public string SaveUpCollectionName { get; set; } = null!;
    }

    public class CollectionSettings
    {
        public string CollectionName { get; set; } = null!;
    }

}
