using ApiKeyCustomAttributes.Attributes;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiKey]
    [ApiController]
    public class TempIndoorAverageController : ControllerBase
    {
        #region Prop und Kunstruktor
        private ITempIndoorAverage _weather;

        public TempIndoorAverageController(ITempIndoorAverage weather)
        {
            _weather = weather;
        }
        #endregion

        /// <summary>
        /// Ruft den durchschnittlichen Innenraumtemperaturwert für einen bestimmten Tag und eine bestimmte Uhrzeit ab.
        /// </summary>
        /// <param name="dayTime">Der Tag und die Uhrzeit, für die der Innenraumtemperaturwert abgerufen werden soll.</param>
        /// <returns>Der durchschnittliche Innenraumtemperaturwert für den angegebenen Tag und die angegebene Uhrzeit.</returns>
        /// <remarks>Falls kein Wert für den angegebenen Tag und die angegebene Uhrzeit gefunden wird, wird NotFound zurückgegeben.</remarks>

        [HttpGet("{ID}")]
        public ActionResult<IndoorTempAveregaModel> Get(string dayTime)
        {
            try
            {
                IndoorTempAveregaModel weathers = _weather.Get(dayTime);
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
        /// Fügt eine neue Indoor Temperatur Durchschnittsdatensatz hinzu.
        ///</summary>
        ///<param name="weather">Indoor Temperatur Durchschnittsdatensatz.</param>
        ///<returns>Nichts wird zurückgegeben.</returns>
        [HttpPost]
        public void Post(IndoorTempAveregaModel weather)
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
