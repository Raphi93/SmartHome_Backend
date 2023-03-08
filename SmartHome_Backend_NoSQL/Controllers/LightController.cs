using ApiKeyCustomAttributes.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class LightController : ControllerBase
    {
        private ILamps _lamps;

        public LightController(ILamps lamp)
        {
            _lamps = lamp;
        }

        [HttpGet]
        public ActionResult<List<LichterModels>> GetAll()
        {
            try
            {
                return _lamps.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        [HttpGet("name")]
        public ActionResult<LichterModels> Get(string name)
        {
            try
            {
                LichterModels lamp = _lamps.Get(name);
                if (lamp == null)
                    return NotFound();
                return lamp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        [HttpPost]
        public IActionResult Post(LichterModels lamp)
        {
            try
            {
                _lamps.Add(lamp);
                return CreatedAtAction(nameof(Get), new { name = lamp.Name }, lamp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        [HttpPut("name")]
        public IActionResult Update(string name, LichterModels lamp)
        {
            try
            {
                var light = _lamps.Get(name);

                if (light is null)
                {
                    return NotFound();
                }

                lamp.Name = light.Name;
                _lamps.Update(name, lamp);
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
