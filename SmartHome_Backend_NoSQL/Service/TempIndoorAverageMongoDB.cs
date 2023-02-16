using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public class TempIndoorAverageMongoDB : ITempIndoorAverage
    {
        #region Prop und Kunstrucktor
        private readonly IMongoCollection<IndoorTempAveregaModel> _average;

        public TempIndoorAverageMongoDB(IOptions<WeatherStationDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wsDatabaseSettings.Value.DatabaseName);

            _average = mongoDatabase.GetCollection<IndoorTempAveregaModel>(
               wsDatabaseSettings.Value.TempAverageCollectionName);
        }
        #endregion
        public void Add(IndoorTempAveregaModel weather)
        {
            try
            {
                _average.InsertOne(weather);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }

        public IndoorTempAveregaModel Get(string dayTime)
        {
            try
            {
                return _average.Find(x => x.daytime == dayTime).FirstOrDefault();
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }
    }
}
