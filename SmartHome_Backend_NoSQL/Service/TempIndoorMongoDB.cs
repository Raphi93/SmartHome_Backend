﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;
using System;

namespace SmartHome_Backend_NoSQL.Service
{
    public class TempIndoorMongoDB : ITempIndoor
    {
        #region Prop und Kunstrucktor
        private readonly IMongoCollection<IndoorTempModel> _temp;
        private readonly IMongoCollection<IndoorTempAveregaModel> _average;

        public TempIndoorMongoDB(IOptions<WeatherStationDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wsDatabaseSettings.Value.DatabaseName);

            _temp = mongoDatabase.GetCollection<IndoorTempModel>(
               wsDatabaseSettings.Value.TempCollectionName);

            _average = mongoDatabase.GetCollection<IndoorTempAveregaModel>(
               wsDatabaseSettings.Value.TempAverageCollectionName);
        }
        #endregion

        public void Add(IndoorTempModel weather)
        {
            var get = _temp.Find(x => true).Count();
            if (get == null)
            {
                get = 0;
            }
            int id = Convert.ToInt32(get);
            if (get != 0) {
                AverageCalc(id, weather);
            }
            id++;
            weather.floorTempMin = weather.floorTemp;
            weather.floorTempMax = weather.floorTemp;
            weather.wallTempMin = weather.wallTemp;
            weather.wallTempMax = weather.wallTemp;
            weather.id = id;
            try
            {
                _temp.InsertOne(weather);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }

        public IndoorTempModel Get(string dayTime)
        {
            try
            {
                return _temp.Find(x => x.daytime == dayTime).FirstOrDefault();
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }

        public List<IndoorTempModel> GetAll()
        {
            try
            {
                List<IndoorTempModel> get = _temp.Find(_ => true).ToList();

                return get;
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return null;
            }
        }

        public void Update(string dayTime, IndoorTempModel weather)
        {
            try
            {
                var get = _temp.Find(x => x.daytime == dayTime).FirstOrDefault();
                if (get != null)
                {
                    weather._id = get._id;
                    weather.id = get.id;
                    weather.wallTempMax = CalcWallsTempMax(dayTime, weather);
                    weather.wallTempMin = CalcWallsTempMin(dayTime, weather);
                    weather.floorTempMax = CalcFloorTempMax(dayTime, weather);
                    weather.floorTempMin = CalcFloorTempMin(dayTime, weather);
                    _temp.ReplaceOne(x => x.daytime == dayTime, weather);

                }
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return;
            }
        }
        public float CalcFloorTempMax(string dayTime, IndoorTempModel weather)
        {
            var get = _temp.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.floorTempMax < weather.floorTemp)
            {
                return (float)weather.floorTemp;
            }
            else
            {
                return (float)get.floorTempMax;
            }
        }

        public float CalcFloorTempMin(string dayTime, IndoorTempModel weather)
        {
            var get = _temp.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.floorTempMin > weather.floorTemp)
            {
                return (float)weather.floorTemp;
            }
            else
            {
                return (float)get.floorTempMin;
            }
        }
        public float CalcWallsTempMax(string dayTime, IndoorTempModel weather)
        {
            var get = _temp.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.wallTempMax < weather.wallTemp)
            {
                return (float)weather.wallTemp;
            }
            else
            {
                return (float)get.wallTempMax;
            }
        }

        public float CalcWallsTempMin(string dayTime, IndoorTempModel weather)
        {
            var get = _temp.Find(x => x.daytime == dayTime).FirstOrDefault();
            if (get.wallTempMin > weather.wallTemp)
            {
                return (float)weather.wallTemp;
            }
            else
            {
                return (float)get.wallTempMin;
            }
        }
        public void AverageCalc(int id, IndoorTempModel weather)
        {
            var yesterday = _temp.Find(x => x.id == id).FirstOrDefault();
            if (weather != null)
            {
                var count = _average.Find(x => true).Count();
                if (count != 0)
                {
                    double averageFloorTemp = Convert.ToDouble(_average.AsQueryable().Average(r => r.floorTemp));
                    double averageWallTemp = Convert.ToDouble(_average.AsQueryable().Average(r => r.wallTemp));
                    weather.floorTemp = averageFloorTemp;
                    weather.wallTemp = averageWallTemp;

                    Update(yesterday.daytime, weather);

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
