using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
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

        // GET api/<WeatherStationController>/5
        [HttpGet("{ID}")]
        public ActionResult<WeatherAverageModel> Get(string _id)
        {
            try
            {
                WeatherAverageModel weathers = _weather.Get(_id);
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
