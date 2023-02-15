using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
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

        // GET: api/<WeatherStationController>
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

        // GET api/<WeatherStationController>/5
        [HttpGet("Datum")]
        public ActionResult<WeatherSationModel> Get(string date)
        {
            try
            {
                WeatherSationModel weathers = _weather.Get(date);
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

        // PUT api/<WeatherStationController>/5
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
