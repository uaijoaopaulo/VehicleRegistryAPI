using Microsoft.AspNetCore.Mvc;

namespace VehicleRegistry.Api.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        [HttpGet, Route("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
}
