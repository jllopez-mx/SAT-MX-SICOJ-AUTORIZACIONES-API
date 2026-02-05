using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AutorizacionesAPI.Api.V1.Controllers
{
    [Route("sicoj/autorizaciones/api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public IActionResult Status()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            Version version = name.Version!;
            return Ok($"Assembly: {name.Name} - Versión: {version}");
        }
    }
}
