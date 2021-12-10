
using Microsoft.AspNetCore.Mvc;

namespace NihFix.Ocelot.MixedRouting.Example.ApiFromDiscovery
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello from service discovery!");
        }
    }
}