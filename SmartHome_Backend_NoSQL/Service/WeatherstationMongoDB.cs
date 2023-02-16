using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;
using System;

namespace SmartHome_Backend_NoSQL.Service
{
    public class WeatherstationMongoDB : IWeatherstation
    {

        #region Prop und Kunstrucktor
        private readonly IMongoCollection<WeatherSationModel> _weather;
        private readonly IMongoCollection<WeatherAverageModel> _average;

        public WeatherstationMongoDB(IOptions<WeatherStationDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wsDatabaseSettings.Value.DatabaseName);

            _weather = mongoDatabase.GetCollection<WeatherSationModel>(
               wsDatabaseSettings.Value.ServicesCollectionName);

            _average = mongoDatabase.GetCollection<WeatherAverageModel>(
               wsDatabaseSettings.Value.AverageCollectionName);
        }
        #endregion

        public List<WeatherSationModel> GetAll()
        {
            try
            {
                List<WeatherSationModel> get = _weather.Find(_ => true).ToList();

                return get;
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }

        public WeatherSationModel Get(string daytime)
        {
            try
            {
                return _weather.Find(x => x.daytime == daytime).FirstOrDefault();
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }

        public void Add(WeatherSationModel weather)
        {
            var get = _weather.Find(x => true).Count();
            if (get == null)
            {
                get = 1;
            }
            int id = Convert.ToInt32(get);
            AverageCalc(id, weather);
            id++;
            weather.tempMin = weather.temp;
            weather.tempMax = weather.temp;
            weather.windMin = weather.wind;
            weather.windMax = weather.wind;
            weather.humidityMax = weather.humidity;
            weather.humidityMin = weather.humidity;
            weather.id= id;

            try
            {
                _weather.InsertOne(weather);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }

        public void Update(string dayTime, WeatherSationModel weather)
        {
            try
            {
                var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
                if (get != null)
                {
                    int id = Convert.ToInt32(get.id);
                    if (id > 0)
                    {
                        --id;
                        var yesterday = _weather.Find(x => x.id == id).FirstOrDefault();

                        weather._id = get._id;
                        weather.id = get.id;
                        weather.tempMax = CalcTempMax(dayTime, weather);
                        weather.tempMin = CalcTempMin(dayTime, weather);
                        weather.windMax = CalcWindMax(dayTime, weather);
                        weather.windMin = CalcWindMin(dayTime, weather);
                        weather.humidityMax = CalcHumidityMax(dayTime, weather);
                        weather.humidityMin = CalcHumidityMin(dayTime, weather);
                        weather.sunDuration = weather.sunDuration - (14238 - yesterday.sunDuration);
                        weather.rain = weather.rain - (803 - yesterday.sunDuration);
                        if (get.raining == true)
                            weather.raining = true;
                        _weather.ReplaceOne(x => x.daytime == dayTime, weather);
                    }
                }
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }

        private double? CalcHumidityMax(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.humidityMax < weather.humidity)
            {
                return (float)weather.humidity;
            }
            else
            {
                return (float)get.humidityMax;
            }
        }

        private double? CalcHumidityMin(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.humidityMin > weather.humidity)
            {
                return (float)weather.humidity;
            }
            else
            {
                return (float)get.humidityMin;
            }
        }

        public float CalcTempMax(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.tempMax < weather.temp)
            {
                return (float)weather.temp;
            }
            else
            {
                return (float)get.tempMax;
            }
        }

        public float CalcTempMin(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.tempMin > weather.temp)
            {
                return (float)weather.temp;
            }
            else
            {
                return (float)get.tempMin;
            }
        }

        public float CalcWindMax(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.windMax < weather.wind)
            {
                return (float)weather.wind;
            }
            else
            {
                return (float)get.windMax;
            }
        }

        public float CalcWindMin(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.windMin > weather.wind)
            {
                return (float)weather.wind;
            }
            else
            {
                return (float)get.windMin;
            }
        }

        public void AverageCalc(int id, WeatherSationModel weathers)
        {
            var yesterday = _weather.Find(x => x.id == id).FirstOrDefault();
            if (weathers != null)
            {
                var count = _average.Find(x => true).Count();
                if (count != 0)
                {
                    var averageTemp = _average.AsQueryable().Average(r => r.temp);
                    var averageWind = _average.AsQueryable().Average(r => r.wind);
                    var averageHumidity = _average.AsQueryable().Average(r => r.humidity);
                    weathers.temp = averageTemp;
                    weathers.wind = averageWind;
                    weathers.humidity = averageHumidity;
                    weathers.daytime = yesterday.daytime;
                    weathers.rain = yesterday.rain;
                    weathers.raining = yesterday.raining;
                    weathers.id = id;
                    weathers._id = yesterday._id;
                    Update(yesterday.daytime, weathers);

                    _average.DeleteMany(x => true);
                }
            }
            else
            {
                Console.WriteLine($"Could not find weather document with ID {id}");
            }

        }
    }
}
