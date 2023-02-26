using ApiKeyCustomAttributes.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiKey]
    [ApiController]
    public class TempIndoorController : ControllerBase
    {
        #region Prop und Kunstruktor
        private ITempIndoor _ondoor;

        public TempIndoorController(ITempIndoor weather)
        {
            _ondoor = weather;
        }
        #endregion

        /// <summary>
        /// Gibt alle gespeicherten Innenraumtemperaturdaten zurück.
        /// </summary>
        /// <returns>Liste von Innenraumtemperaturdaten</returns>
        [HttpGet]
        public ActionResult<List<IndoorTempModel>> GetAll()
        {
            try
            {
                return _ondoor.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        ///<summary>
        /// Holt den Indoor-Temperatur-Eintrag für das angegebene Datum ab.
        ///</summary>
        ///<param name="dayTime">Datum des gesuchten Eintrags.</param>
        ///<returns>Gibt den gefundenen Indoor-Temperatur-Eintrag zurück, falls vorhanden, andernfalls NotFound.</returns>
        [HttpGet("Datum")]
        public ActionResult<IndoorTempModel> Get(string dayTime)
        {
            try
            {
                IndoorTempModel weathers = _ondoor.Get(dayTime);
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
        ///Fügt eine neue IndoorTemperaturmessung hinzu und gibt einen HTTP 201 Created Statuscode zurück.
        ///</summary>
        ///<remarks>
        ///Die Methode empfängt ein IndoorTempModel Objekt, fügt es der Datenbank hinzu und gibt das hinzugefügte Objekt zurück.
        ///Wenn ein Fehler auftritt, gibt die Methode einen HTTP 404 Not Found Statuscode zurück.
        ///</remarks>
        ///<param name="weather">Das IndoorTempModel Objekt, das hinzugefügt werden soll.</param>
        ///<returns>Ein HTTP 201 Created Statuscode zusammen mit dem hinzugefügten IndoorTempModel Objekt.</returns>
        [HttpPost]
        public IActionResult Post(IndoorTempModel weather)
        {
            try
            {
                _ondoor.Add(weather);
                return CreatedAtAction(nameof(Get), new { dayTime = weather.daytime }, weather);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        /// <summary>
        /// Aktualisiert eine Indoor-Temperatur auf Grundlage eines Datums.
        /// </summary>
        /// <param name="dayTime">Das Datum, auf das sich die zu aktualisierende Temperatur bezieht.</param>
        /// <param name="weather">Das aktualisierte Temperaturmodell.</param>
        /// <returns>Kein Inhalt (NoContent) oder NotFound, wenn das Temperaturmodell nicht gefunden wurde.</returns>
        [HttpPut("Datum")]
        public IActionResult Update(string dayTime, IndoorTempModel weather)
        {
            try
            {
                var weathers = _ondoor.Get(dayTime);

                if (weathers is null)
                {
                    return NotFound();
                }

                weather.daytime = weathers.daytime;
                _ondoor.Update(dayTime, weather);
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
