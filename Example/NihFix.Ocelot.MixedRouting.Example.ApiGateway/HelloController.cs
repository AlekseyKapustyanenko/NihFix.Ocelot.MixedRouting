using Microsoft.AspNetCore.Mvc;

namespace NihFix.Ocelot.MixedRouting.Example.ApiGateway
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello from config defined!");
        }
    }
}