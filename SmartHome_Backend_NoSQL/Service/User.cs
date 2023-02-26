using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public class User : IUser
    {
        #region Prop und Kunstrucktor
        private readonly IMongoCollection<UserModel> _user;
        private const string APIKEYNAME = "ApiKey";

        public User(IOptions<WeatherStationDataBaseSetting> wsDatabaseSettings)
        {
            var mongoClient = new MongoClient(wsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(wsDatabaseSettings.Value.DatabaseName);

            _user = mongoDatabase.GetCollection<UserModel>(
               wsDatabaseSettings.Value.UserCollectionName);
        }
        #endregion

        /// <summary>
        /// Gibt den API-Schlüssel zurück, wenn der Benutzer autorisiert ist.
        /// </summary>
        /// <returns>HttpResponseMessage, das den API-Schlüssel enthält, wenn autorisiert, oder eine 401 Unauthorized-Antwort, wenn nicht autorisiert</returns>
        /// <param name="user">Benutzermodell mit Namen und Passwort des Benutzers</param>
        /// <param name="configuration">Instanz der IConfiguration-Klasse, die den API-Schlüssel enthält</param>
        public string Post(UserModel user, IConfiguration configuration)
        {
            try
            {
                var existingUser = _user.Find(x => x.name == user.name).FirstOrDefault();

                if (existingUser != null && existingUser.Passwort == user.Passwort)
                {
                    var apiKey = configuration.GetValue<string>(APIKEYNAME);
                    return apiKey.ToString();
                }
                else
                {
                    return "Falsche Angaben";
                }
            }
            catch (MongoException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
