using ApiKeyCustomAttributes.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiKey]
    [ApiController]
    public class WeatherAverageController : ControllerBase
    {

        #region Prop und Kunstruktor
        private IWeatherAverage _weather;

        public WeatherAverageController (IWeatherAverage weather)
        {
            _weather = weather;
        }
        #endregion

        /// <summary>
        /// Gibt den durchschnittlichen Wetterdaten für den angegebenen Tag zurück.
        /// </summary>
        /// <param name="dayTime">Das Datum, für das die Wetterdaten abgerufen werden sollen.</param>
        /// <returns>Die durchschnittlichen Wetterdaten für den angegebenen Tag.</returns>
        /// <remarks>Wenn keine Wetterdaten für den angegebenen Tag gefunden werden können, wird NotFound zurückgegeben.</remarks>
        [HttpGet("{ID}")]
        public ActionResult<WeatherAverageModel> Get(string dayTime)
        {
            try
            {
                WeatherAverageModel weathers = _weather.Get(dayTime);
                if (weathers == null)
                    return NotFound();
                return weathers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        ///<summary>
        /// Erstellt eine neue Wetterdurchschnittsdatensatz im System.
        ///</summary>
        ///<param name="weather">Die Wetterdurchschnittsdatensatz, die erstellt werden soll.</param>
        ///<remarks>Die Wetterdurchschnittsdatensatz wird in der Datenbank gespeichert.</remarks>
        [HttpPost]
        public void Post(WeatherAverageModel weather)
        {
            try
            {
                _weather.Add(weather);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
            }
        }
    }
}
