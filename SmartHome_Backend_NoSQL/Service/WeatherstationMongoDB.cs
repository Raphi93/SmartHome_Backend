using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;
using System;
using System.Linq;

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

        public WeatherstationMongoDB()
        {
        }
        #endregion

        /// <summary>
        /// GetAll bringt alle daten 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Bringt die Daten mit dem Datum
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ADD Mtehode wo eine neue zeile einfügt
        /// </summary>
        /// <param name="weather"></param>
        public void Add(WeatherSationModel weather)
        {
            var get = _weather.Find(x => true).Count();
 
            int id = Convert.ToInt32(get);
            if (id != 0)

            {
                var oldDuk = _weather.Find(x => x.id == id).FirstOrDefault();
                weather.sunDurSOP = oldDuk.sunDurSOP;
                weather.rainDurSOP = oldDuk.rainDurSOP;
            }
            id++;
            weather._id = "";
            weather.tempMin = weather.temp;
            weather.tempMax = weather.temp;
            weather.windMin = weather.wind;
            weather.windMax = weather.wind;
            weather.humidityMax = weather.humidity;
            weather.humidityMin = weather.humidity;
            weather.id = id;

            double sun = (double)weather.sunDuration;
            weather.sunDuration = (60 * sun);

            try
            {
                _weather.InsertOne(weather);
                id--;
                if (id > 1)
                {
                    AverageCalc(id, weather);
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }


        /// <summary>
        /// Update wo die daten Veränder
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="weather"></param>
        public void Update(string dayTime, WeatherSationModel weather)
        {
            try
            {
                var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault();
                if (get != null)
                {
                    int id = Convert.ToInt32(get.id);
                    if (id > 1)
                    {
                        weather.rain = CheckRain(weather, dayTime);
                        weather.sunDuration = CheckSunDur(weather, dayTime);
                    }
                    else
                    {
                        double sun = (double)weather.sunDuration;
                        sun = sun / 60;
                        weather.sunDuration = sun;
                        weather.rain = weather.rain;
                    }
                    weather._id = get._id;
                    weather.id = get.id;
                    weather.tempMax = CalcTempMax(dayTime, weather);
                    weather.tempMin = CalcTempMin(dayTime, weather);
                    weather.windMax = CalcWindMax(dayTime, weather);
                    weather.windMin = CalcWindMin(dayTime, weather);
                    weather.humidityMax = CalcHumidityMax(dayTime, weather);
                    weather.humidityMin = CalcHumidityMin(dayTime, weather);
                    weather.sunDurSOP = get.sunDurSOP;
                    weather.rainDurSOP = get.rainDurSOP;
                    if (get.raining == true)
                        weather.raining = true;
                    _weather.ReplaceOne(x => x.daytime == dayTime, weather);
                }
                else
                {
                    Console.WriteLine($"Wurde nicht Gefunden mit ID {weather.id}");
                }
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }

        /// <summary>
        /// Check SonneDauer wo die daten Prüft und das richtige resultat zurückgibt
        /// </summary>
        /// <param name="weather"></param>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        private double CheckSunDur(WeatherSationModel weather, string dayTime)
        {
            if (weather.sunDuration < 10)
            { 
                int suny = Convert.ToInt32(weather.sunDurSOP);
                suny++;
                weather.sunDurSOP = suny;
            }

            double lastDaySun = (Double)(_weather.Find(x => x.daytime != dayTime).ToList().Sum(x => x.sunDuration));
            double lastDayMin = lastDaySun * 60 + (1600 * weather.sunDurSOP);
            double sun = (Double)(weather.sunDuration - lastDayMin);
            sun = sun / 60;
            return sun;
        }

        /// <summary>
        /// Prüft wegen des Regens dases Eine Korecktes Resultate zurückgibt
        /// </summary>
        /// <param name="weather"></param>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public double CheckRain(WeatherSationModel weather, string dayTime)
        {
            if (weather.rain < 10)
            {
                int rains = Convert.ToInt32(weather.rainDurSOP);
                rains++;
                weather.rainDurSOP = rains;
            }

            var lastRain = _weather.Find(x => x.daytime != dayTime).ToList().Sum(x => x.rain);
            double rain = (double)(weather.rain - lastRain) + (1600 * weather.sunDurSOP); ;
            return rain;
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
            if (yesterday != null)
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
                    weathers.rain = weathers.rain;
                    weathers.raining = yesterday.raining;
                    weathers.sunDuration = weathers.sunDuration;
                    weathers.daytime = yesterday.daytime;
                    weathers.id = id;
                    weathers._id = yesterday._id;
                    Update(yesterday.daytime, weathers);
                    _average.DeleteMany(x => true);
                }
            }
            else
            {
                Console.WriteLine($"Wurde nicht Gefunden mit ID {id}");
            }
        }
    }
}
