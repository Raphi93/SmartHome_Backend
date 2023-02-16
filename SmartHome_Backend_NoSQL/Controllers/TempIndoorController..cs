using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
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

        // GET: api/<WeatherStationController>
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

        // GET api/<WeatherStationController>/5
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

        // POST api/<WeatherStationController>
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

        // PUT api/<WeatherStationController>/5
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
