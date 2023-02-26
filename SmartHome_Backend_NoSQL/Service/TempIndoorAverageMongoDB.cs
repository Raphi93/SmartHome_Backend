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

        /// <summary>
        /// Fügt ein neues IndoorTempAveregaModel-Objekt hinzu.
        /// </summary>
        /// <param name="weather">Das Objekt, das die Temperaturdaten enthält.</param>
        /// <remarks>Wirft eine NullReferenceException, wenn das übergebene Objekt Null ist.</remarks>
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

        /// <summary>
        /// Gibt das IndoorTempAveregaModel-Objekt zurück, das dem angegebenen Tag und Zeitpunkt entspricht.
        /// </summary>
        /// <param name="dayTime">Der Tag und Zeitpunkt, für den das IndoorTempAveregaModel-Objekt abgerufen werden soll.</param>
        /// <returns>Das IndoorTempAveregaModel-Objekt, das dem angegebenen Tag und Zeitpunkt entspricht, oder Null, wenn kein entsprechendes Objekt gefunden wurde.</returns>
        /// <remarks>Wirft eine MongoException, wenn ein Fehler beim Zugriff auf die Datenbank auftritt.</remarks>
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
