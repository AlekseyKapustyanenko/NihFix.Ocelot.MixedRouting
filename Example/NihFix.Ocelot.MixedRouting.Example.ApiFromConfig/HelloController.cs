using Microsoft.AspNetCore.Mvc;

namespace NihFix.Ocelot.MixedRouting.Example.ApiFromConfig
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