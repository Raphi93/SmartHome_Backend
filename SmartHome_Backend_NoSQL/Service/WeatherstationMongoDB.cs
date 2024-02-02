using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Controllers;
using SmartHome_Backend_NoSQL.Models;
using System;
using System.Linq;
using WeatherAverageModel = SmartHome_Backend_NoSQL.Controllers.WeatherAverageModel;

namespace SmartHome_Backend_NoSQL.Service
{
    public class WeatherstationMongoDB : IWeatherstation
    {

        #region Prop und Kunstrucktor
        private readonly IMongoCollection<WeatherSationModel> _weather;
        private readonly IMongoCollection<WeatherAverageModel> _average;

        public WeatherstationMongoDB(IOptions<SmartHomeDataBaseSetting> wsDatabaseSettings)
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

        /// <summary>
        /// Ruft alle Wetterstation-Datensätze aus der Datenbank ab.
        /// </summary>
        /// <returns>Eine Liste von Wetterstation-Modellen oder null, wenn ein Fehler aufgetreten ist.</returns>
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
        /// Ruft das WeatherSationModel-Objekt für den angegebenen Zeitpunkt ab.
        /// </summary>
        /// <param name="daytime">Der Zeitpunkt, für den das Wetter abgerufen werden soll.</param>
        /// <returns>Das WeatherSationModel-Objekt für den angegebenen Zeitpunkt oder null, wenn ein Fehler auftritt.</returns>
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
        /// Fügt ein neues WeatherSationModel-Objekt hinzu.
        /// </summary>
        /// <param name="weather">Das WeatherSationModel-Objekt, das hinzugefügt werden soll.</param>
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
            weather.sunDuration = (sun / 60);

            try
            {
                _weather.InsertOne(weather);
                id--;
                if (id > 0)
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
        /// Aktualisiert das WeatherSationModel-Objekt mit der angegebenen "dayTime".
        /// </summary>
        /// <param name="dayTime">Die "dayTime" des zu aktualisierenden Objekts.</param>
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

        ///<summary>
        ///Diese Methode überprüft, ob die Sonnenscheindauer an einem bestimmten Tag kürzer als 5 Minuten ist.
        ///Wenn ja, wird die Anzahl der sonnigen Tage erhöht.
        ///Anschließend wird die Sonnenscheindauer des aktuellen Tages berechnet, indem die Sonnenscheindauer
        ///des Vortages sowie die Anzahl der sonnigen Tage seit dem letzten Regen berücksichtigt werden.
        ///</summary>
        ///<param name="weather">Das Wetterobjekt, dessen Sonnenscheindauer überprüft wird.</param>
        ///<param name="dayTime">Die Zeit des Tages, an dem das Wetterobjekt gemessen wurde.</param>
        ///<returns>Die berechnete Sonnenscheindauer des aktuellen Tages in Stunden.</returns>
        private double CheckSunDur(WeatherSationModel weather, string dayTime)
        {
            int suny = Convert.ToInt32(weather.sunDurSOP);
            if (weather.sunDuration < 5)
            { 
                suny++;
                weather.sunDurSOP = suny;
            }

            double lastDaySun = (Double)(_weather.Find(x => x.daytime != dayTime).ToList().Sum(x => x.sunDuration));
            double lastDayMin = lastDaySun * 60 + (1600 * weather.sunDurSOP);
            double sun = (Double)(weather.sunDuration - lastDayMin);
            if (sun < 0)
            {
                suny--;
                weather.sunDurSOP = suny;
                sun = sun + 1600;
            }
            sun = sun / 60;
            return sun;
        }

        ///<summary>
        ///Diese Methode berechnet die Regenmenge für den aktuellen Tag basierend auf den
        ///bisherigen Niederschlagsdaten und der Sonnenscheindauer.
        ///</summary>
        ///<param name="weather">Das Wetterobjekt, dessen Regenmenge berechnet wird.</param>
        ///<param name="dayTime">Die Zeit des Tages, an dem das Wetterobjekt gemessen wurde.</param>
        ///<returns>Die berechnete Regenmenge des aktuellen Tages in Millimetern.</returns>
        public double CheckRain(WeatherSationModel weather, string dayTime)
        {
            int rains = Convert.ToInt32(weather.rainDurSOP);
            if (weather.rain < 1)
            {
                rains++;
                weather.rainDurSOP = rains;
            }

            var lastRain = _weather.Find(x => x.daytime != dayTime).ToList().Sum(x => x.rain);
            double rain = (double)(weather.rain - lastRain) + (1600 * weather.rainDurSOP);

            if (rain < 0)
            {
                rains--;
                weather.sunDurSOP = rains;
                rain = rain + 1600;
            }
            return rain;
        }


        ///<summary>
        ///Diese Methode berechnet die maximale Luftfeuchtigkeit an einem bestimmten Tag.
        ///</summary>
        ///<param name="dayTime">Die Zeit des Tages, für den die maximale Luftfeuchtigkeit berechnet wird.</param>
        ///<param name="weather">Das Wetterobjekt, das die aktuellen Luftfeuchtigkeitsdaten enthält.</param>
        ///<returns>Die berechnete maximale Luftfeuchtigkeit des Tages oder null, wenn keine Daten verfügbar sind.</returns>
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

        ///<summary>
        ///Diese Methode berechnet die minimale Luftfeuchtigkeit an einem bestimmten Tag.
        ///</summary>
        ///<param name="dayTime">Die Zeit des Tages, für den die minimale Luftfeuchtigkeit berechnet wird.</param>
        ///<param name="weather">Das Wetterobjekt, das die aktuellen Luftfeuchtigkeitsdaten enthält.</param>
        ///<returns>Die berechnete minimale Luftfeuchtigkeit des Tages oder null, wenn keine Daten verfügbar sind.</returns>
        private double? CalcHumidityMin(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault(); // Abrufen der Wetterdaten für den angegebenen Tag
            if (get.humidityMin > weather.humidity) // Überprüfen, ob die aktuelle Luftfeuchtigkeit kleiner als die bisherige minimale Luftfeuchtigkeit ist
            {
                return (float)weather.humidity; // Rückgabe der aktuellen Luftfeuchtigkeit als neue minimale Luftfeuchtigkeit
            }
            else
            {
                return (float)get.humidityMin; // Rückgabe der bisherigen minimalen Luftfeuchtigkeit
            }
        }


        ///<summary>
        ///Diese Methode berechnet die maximale Temperatur an einem bestimmten Tag.
        ///</summary>
        ///<param name="dayTime">Die Zeit des Tages, für den die maximale Temperatur berechnet wird.</param>
        ///<param name="weather">Das Wetterobjekt, das die aktuellen Temperaturdaten enthält.</param>
        ///<returns>Die berechnete maximale Temperatur des Tages.</returns>
        public float CalcTempMax(string dayTime, WeatherSationModel weather)
        {
            var get = _weather.Find(x => x.daytime == dayTime).FirstOrDefault(); // Abrufen der Wetterdaten für den angegebenen Tag
            if (get.tempMax < weather.temp) // Überprüfen, ob die aktuelle Temperatur größer als die bisherige maximale Temperatur ist
            {
                return (float)weather.temp; // Rückgabe der aktuellen Temperatur als neue maximale Temperatur
            }
            else
            {
                return (float)get.tempMax; // Rückgabe der bisherigen maximalen Temperatur
            }
        }

        ///<summary>
        ///Diese Methode berechnet die minimale Temperatur für einen bestimmten Tag.
        ///Sie vergleicht die aktuelle Temperatur mit der minimalen Temperatur des Vortages und gibt das Minimum zurück.
        ///</summary>
        ///<param name="dayTime">Die Zeit des Tages, für den die minimale Temperatur berechnet wird.</param>
        ///<param name="weather">Das Wetterobjekt, dessen Temperatur überprüft wird.</param>
        ///<returns>Die minimale Temperatur für den angegebenen Tag.</returns>
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

        ///<summary>
        ///Diese Methode berechnet die maximale Windgeschwindigkeit für einen bestimmten Tag.
        ///Sie vergleicht die aktuelle Windgeschwindigkeit mit der maximalen Windgeschwindigkeit des Vortages und gibt das Maximum zurück.
        ///</summary>
        ///<param name="dayTime">Die Zeit des Tages, für den die maximale Windgeschwindigkeit berechnet wird.</param>
        ///<param name="weather">Das Wetterobjekt, dessen Windgeschwindigkeit überprüft wird.</param>
        ///<returns>Die maximale Windgeschwindigkeit für den angegebenen Tag.</returns>
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

        ///<summary>
        ///Diese Methode berechnet die minimale Windgeschwindigkeit an einem bestimmten Tag.
        ///Es wird das Wetterobjekt mit der angegebenen dayTime aus der Liste abgerufen und verglichen,
        ///ob die minimale Windgeschwindigkeit im aktuellen Wetterobjekt kleiner ist.
        ///Falls dies zutrifft, wird der Wert aus dem aktuellen Wetterobjekt zurückgegeben, ansonsten der Wert aus der Liste.
        ///</summary>
        ///<param name="dayTime">Die Zeit des Tages, an dem das Wetterobjekt gemessen wurde.</param>
        ///<param name="weather">Das Wetterobjekt, dessen minimale Windgeschwindigkeit berechnet wird.</param>
        ///<returns>Die minimale Windgeschwindigkeit des Tages.</returns>
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

        /// <summary>
        /// Berechnet den Durchschnittswert der Temperatur, Windgeschwindigkeit und Luftfeuchtigkeit der letzten Wetteraufzeichnungen
        /// und aktualisiert die gegebene Wetteraufzeichnung mit diesen Werten. Löscht dann alle Einträge aus der Average Collection.
        /// </summary>
        /// <param name="id">Die ID des zu aktualisierenden Wetters.</param>
        /// <param name="weathers">Das zu aktualisierende Wetterobjekt.</param>
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

                    weathers.tempMin = yesterday.tempMin;
                    weathers.tempMax = yesterday.tempMax;
                    weathers.windMin = yesterday.windMin;
                    weathers.windMax = yesterday. windMax;
                    weathers.humidityMin = yesterday.humidityMin;
                    weathers.humidityMax = yesterday.humidityMax;
                    weathers.daytime = yesterday.daytime;
                    weathers.rain = yesterday.rain;
                    weathers.raining = yesterday.raining;
                    weathers.sunDuration = yesterday.sunDuration;
                    weathers.daytime = yesterday.daytime;
                    weathers.rainDurSOP = yesterday.rainDurSOP;
                    weathers.sunDurSOP = yesterday.sunDurSOP;
                    weathers.id = id;
                    weathers._id = yesterday._id;
                    _weather.ReplaceOne(x => x.daytime == yesterday.daytime, weathers);
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
