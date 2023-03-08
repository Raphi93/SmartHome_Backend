using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public class SaveUp_MongoDB : ISaveUp
    {
        private readonly IMongoCollection<SaveUP> _saveUp;
        public SaveUp_MongoDB(IOptions<WeatherStationDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wsDatabaseSettings.Value.DatabaseName);

            _saveUp = mongoDatabase.GetCollection<SaveUP>(
               wsDatabaseSettings.Value.SaveUpCollectionName);
        }

        public void Add(SaveUP saveUp)
        {
            try
            {
                _saveUp.InsertOne(saveUp);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }

        public void DeleteAll()
        {
            try
            {
                _saveUp.DeleteMany(x => true);
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
            }
        }

        public void Delete(string id)
        {
            try
            {
                _saveUp.DeleteOne(x => x.ID == id);
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
            }
        }

        public List<SaveUP> GetAll()
        {
            try
            {
                List<SaveUP> get = _saveUp.Find(_ => true).ToList();
                return get;
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }
    }
}
