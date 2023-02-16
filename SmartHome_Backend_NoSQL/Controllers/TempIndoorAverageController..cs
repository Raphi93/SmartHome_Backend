using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
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

        // GET api/<WeatherStationController>/5
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

        // POST api/<WeatherStationController>
        [HttpPost]
        public IActionResult Post(IndoorTempAveregaModel weather)
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
    }
}
