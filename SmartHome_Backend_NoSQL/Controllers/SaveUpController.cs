using ApiKeyCustomAttributes.Attributes;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class SaveUpController : ControllerBase
    {

        private ISaveUp _saveUp;

        public SaveUpController(ISaveUp saveUp)
        {
            _saveUp = saveUp;
        }

        // GET api/<SaveUpController>/5
        [HttpGet("{id}")]
        public ActionResult<List<SaveUP>> GetAll()
        { 
            try
            {
                return _saveUp.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return NotFound($"Error occured");
            }
        }

        // POST api/<SaveUpController>
        [HttpPost]
        public string Post(SaveUP saveUp)
        {
            try
            {
                _saveUp.Add(saveUp);
                return "Erstellt";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
                return $"Error occured";
            }
        }


        // DELETE api/<SaveUpController>/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            try
            {
                _saveUp.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
            }
        }

        public void DeleteAll()
        {
            try
            {
                _saveUp.DeleteAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured, {ex.Message}");
            }
        }
    }
}
