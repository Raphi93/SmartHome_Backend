using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;
using ApiKeyCustomAttributes.Attributes;

namespace SmartHome_Backend_NoSQL.Controllers
{
    
    [Route("api/[controller]")]
    [ApiKey]
    [ApiController]
    public class WeatherStationController : ControllerBase
    {

        #region Prop und Kunstruktor
        private IWeatherstation _weather;

        public WeatherStationController(IWeatherstation weather)
        {
            _weather = weather;
        }
        #endregion

        /// <summary>
        /// Action Methode zum Abrufen aller Wetterstationen
        /// </summary>
        /// <returns>Eine ActionResult-Instanz, die eine Liste von WeatherSationModel-Objekten oder NotFound zurückgibt, wenn ein Fehler aufgetreten ist</returns>
        [HttpGet]
        public ActionResult<List<WeatherSationModel>> GetAll()
        {
            try
            {
                return _weather.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        /// <summary>
        /// Ruft eine Wetterstationsmessung für ein bestimmtes Datum ab.
        /// </summary>
        /// <param name="dayTime">Das Datum der Wetterstationsmessung im Format "YYYY-MM-DD".</param>
        /// <returns>Eine ActionResult-Instanz, die entweder eine WeatherSationModel-Instanz enthält oder NotFound zurückgibt, wenn keine Messung für das angegebene Datum gefunden wurde.</returns>
        /// <remarks>Wirft eine Ausnahme, wenn ein Fehler auftritt.</remarks>
        [HttpGet("Datum")]
        public ActionResult<WeatherSationModel> Get(string dayTime)
        {
            try
            {
                WeatherSationModel weathers = _weather.Get(dayTime);
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

        /// <summary>
        /// Fügt ein neues Wetterobjekt hinzu.
        /// </summary>
        /// <param name="weather">Das Objekt, das die Wetterdaten enthält.</param>
        /// <returns>Ein IActionResult-Objekt, das den Status der Anfrage widerspiegelt.</returns>
        [HttpPost]
        public IActionResult Post(WeatherSationModel weather)
        {
            try
            {
                _weather.Add(weather);
                return CreatedAtAction(nameof(Get), new { dayTime = weather.daytime }, weather);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        /// <summary>
        /// Aktualisiert ein Wetterobjekt für den angegebenen Tag und Zeitpunkt.
        /// </summary>
        /// <param name="dayTime">Der Tag und Zeitpunkt, für den das Wetter aktualisiert werden soll.</param>
        /// <param name="weather">Das Objekt, das die aktualisierten Wetterdaten enthält.</param>
        /// <returns>Ein IActionResult-Objekt, das den Status der Anfrage widerspiegelt.</returns>
        [HttpPut("Datum")]
        public IActionResult Update(string dayTime, WeatherSationModel weather)
        {
            try
            {
                var weathers = _weather.Get(dayTime);

                if (weathers is null)
                {
                    return NotFound();
                }

                weather.daytime = weathers.daytime;
                _weather.Update(dayTime, weather);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }
    }
}
