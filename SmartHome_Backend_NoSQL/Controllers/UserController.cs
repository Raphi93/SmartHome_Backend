using ApiKeyCustomAttributes.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;
using SmartHome_Backend_NoSQL.Service;

namespace SmartHome_Backend_NoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiKey]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        ///<summary>
        ///Erstellt einen neuen Benutzer mit den gegebenen Informationen und generiert einen API-Schlüssel für den Benutzer.
        ///</summary>
        ///<param name="user">Ein Objekt vom Typ UserModel, das die Benutzerinformationen enthält.</param>
        ///<param name="configuration">Ein Objekt vom Typ IConfiguration, das die Konfiguration für die API-Schlüsselerstellung enthält.</param>
        ///<returns>Die HTTP-Antwort mit dem generierten API-Schlüssel.</returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] UserModel user, [FromServices] IConfiguration configuration)
        {
            var apiKey = _userService.Post(user, configuration);

            if (apiKey == null)
            {
                return NotFound();
            }

            return Ok(apiKey.ToString());
        }
    }
}
