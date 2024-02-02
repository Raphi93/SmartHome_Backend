using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public class TempIndoorMongoDB : ITempIndoor
    {
        #region Prop und Kunstrucktor
        private readonly IMongoCollection<IndoorTempModel> _temp;
        private readonly IMongoCollection<IndoorTempAverageModel> _average;

        public TempIndoorMongoDB(IOptions<SmartHomeDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                wsDatabaseSettings.Value.DatabaseName);

            _temp = mongoDatabase.GetCollection<IndoorTempModel>(
               wsDatabaseSettings.Value.TempCollectionName);

            _average = mongoDatabase.GetCollection<IndoorTempAverageModel>(
               wsDatabaseSettings.Value.TempAverageCollectionName);
        }
        #endregion

        /// <summary>
        /// Fügt ein neues IndoorTempModel-Objekt hinzu.
        /// </summary>
        /// <param name="weather">Das Objekt, das die Temperaturdaten enthält.</param>
        /// <remarks>
        /// Das Objekt wird mit einer neuen ID versehen und die Minimal- und Maximaltemperaturwerte für Boden- und Wandtemperaturen werden initialisiert.
        /// Wenn das Objekt erfolgreich hinzugefügt wurde, wird der Durchschnitt der letzten beiden Temperaturen berechnet, falls es mindestens zwei Temperaturdaten gibt.
        /// Wirft eine NullReferenceException, wenn das übergebene Objekt Null ist.
        /// </remarks>
        public void Add(IndoorTempModel weather)
        {
            var get = _temp.Find(x => true).Count();
            int id = Convert.ToInt32(get);
            if (get == null)
            {
                id = 0;
            }
            id++;
            weather._id = "";
            weather.floorTempMin = weather.floorTemp;
            weather.floorTempMax = weather.floorTemp;
            weather.wallTempMin = weather.wallTemp;
            weather.wallTempMax = weather.wallTemp;
            weather.id = id;
            try
            {
                _temp.InsertOne(weather);
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
        /// Gibt das IndoorTempModel-Objekt zurück, das dem angegebenen Tag und Zeitpunkt entspricht.
        /// </summary>
        /// <param name="dayTime">Der Tag und Zeitpunkt, für den das IndoorTempModel-Objekt abgerufen werden soll.</param>
        /// <returns>Das IndoorTempModel-Objekt, das dem angegebenen Tag und Zeitpunkt entspricht, oder Null, wenn kein entsprechendes Objekt gefunden wurde.</returns>
        /// <remarks>Wirft eine MongoException, wenn ein Fehler beim Zugriff auf die Datenbank auftritt.</remarks>
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

        /// <summary>
        /// Gibt eine Liste aller IndoorTempModel-Objekte zurück, die in der Datenbank vorhanden sind.
        /// </summary>
        /// <returns>Eine Liste aller IndoorTempModel-Objekte, die in der Datenbank vorhanden sind.</returns>
        /// <remarks>Wirft eine MongoException, wenn ein Fehler beim Zugriff auf die Datenbank auftritt.</remarks>
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

        ///<summary>
        /// Aktualisiert die Temperaturdaten für einen bestimmten Zeitpunkt im IndoorTemp-Repository.
        ///</summary>
        ///<param name="dayTime">Der Zeitpunkt, für den die Daten aktualisiert werden sollen.</param>
        ///<param name="weather">Die aktualisierten Temperaturdaten.</param>
        ///<returns>Kein Rückgabewert.</returns>
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

        /// <summary>
        /// Berechnet die maximale Bodentemperatur für einen bestimmten Zeitpunkt, indem sie die aktuelle Bodentemperatur mit der gespeicherten maximalen Bodentemperatur vergleicht.
        /// </summary>
        /// <param name="dayTime">Der Zeitpunkt, für den die maximale Bodentemperatur berechnet werden soll.</param>
        /// <param name="weather">Das Wettermodell, das die aktuelle Bodentemperatur enthält.</param>
        /// <returns>Die maximale Bodentemperatur für den angegebenen Zeitpunkt.</returns>
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

        /// <summary>
        /// Berechnet die Mindesttemperatur des Bodens im Zimmer basierend auf den aktuellen Temperaturdaten.
        /// </summary>
        /// <param name="dayTime">Das Datum und die Uhrzeit, zu denen die Temperaturdaten erfasst wurden.</param>
        /// <param name="weather">Das IndoorTempModel-Objekt, das die aktuellen Temperaturdaten enthält.</param>
        /// <returns>Die Mindesttemperatur des Bodens im Zimmer.</returns>
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

        /// <summary>
        /// Berechnet die maximale Temperatur an den Wänden für eine bestimmte Tageszeit
        /// </summary>
        /// <param name="dayTime">Die Tageszeit, für die die maximale Temperatur berechnet werden soll</param>
        /// <param name="weather">Die aktuellen Wetterdaten</param>
        /// <returns>Die maximale Temperatur an den Wänden für die angegebene Tageszeit</returns>
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

        /// <summary>
        /// Berechnet die minimale Wandtemperatur für einen bestimmten Zeitpunkt basierend auf der aktuellen Temperatur und der vorherigen minimalen Wandtemperatur.
        /// </summary>
        /// <param name="dayTime">Die Zeit des Tages, für die die minimale Wandtemperatur berechnet werden soll.</param>
        /// <param name="weather">Ein IndoorTempModel-Objekt, das die aktuelle Temperatur enthält.</param>
        /// <returns>Die berechnete minimale Wandtemperatur.</returns>
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

        /// <summary>
        /// Berechnet den Durchschnitt der Boden- und Wandtemperatur und aktualisiert das Wettermodell.
        /// </summary>
        /// <param name="id">Die ID des Wettermodells.</param>
        /// <param name="weather">Das Wettermodell, das aktualisiert werden soll.</param>
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
                    weather.id = id;
                    weather._id = yesterday._id;

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
