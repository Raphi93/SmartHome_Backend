using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;
using System;

namespace SmartHome_Backend_NoSQL.Service
{
    public class LightsCollectionName : ILamps
    {
        private readonly IMongoCollection<LichterModels> _lichter;
        public LightsCollectionName(IOptions<WeatherStationDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wsDatabaseSettings.Value.DatabaseName);

            _lichter = mongoDatabase.GetCollection<LichterModels>(
               wsDatabaseSettings.Value.LightsCollectionName);
        }

        public void Add(LichterModels lights)
        {
            try
            {
                _lichter.InsertOne(lights);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }

        public LichterModels Get(string name)
        {
            try
            {
                return _lichter.Find(x => x.Name == name).FirstOrDefault();
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }

        public List<LichterModels> GetAll()
        {
            try
            {
                List<LichterModels> get = _lichter.Find(_ => true).ToList();
                return get;
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }

        public void Update(string name, LichterModels lights)
        {
            try
            {
                var get = _lichter.Find(x => x.Name == name).FirstOrDefault();
                if (get != null)
                {
                    lights._id = get._id;
                    _lichter.ReplaceOne(x => x.Name == name, lights);
                }
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }
    }
}
