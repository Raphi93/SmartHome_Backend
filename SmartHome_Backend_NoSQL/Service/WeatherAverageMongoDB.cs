using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;
using System;

namespace SmartHome_Backend_NoSQL.Service
{
    public class WeatherAverageMongoDB : IWeatherAverage
    {

        #region Prop und Kunstrucktor
        private readonly IMongoCollection<WeatherAverageModel> _average;

        public WeatherAverageMongoDB(IOptions<WeatherStationDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wsDatabaseSettings.Value.DatabaseName);

            _average = mongoDatabase.GetCollection<WeatherAverageModel>(
               wsDatabaseSettings.Value.AverageCollectionName);
        }
        #endregion

        /// <summary>
        /// Fügt ein neues Wetterdurchschnittsmodell zur Datenbank hinzu
        /// </summary>
        /// <param name="weather">Das zu speichernde Wetterdurchschnittsmodell</param>
        public void Add(WeatherAverageModel weather)
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

        /// <summary>
        /// Liefert ein WeatherAverageModel anhand eines Tageszeitschlüssels
        /// </summary>
        /// <param name="dayTime">Tageszeitschlüssel</param>
        /// <returns>Das WeatherAverageModel mit dem Tageszeitschlüssel oder null, wenn kein passendes Model gefunden wurde</returns>
        public WeatherAverageModel Get(string dayTime)
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
