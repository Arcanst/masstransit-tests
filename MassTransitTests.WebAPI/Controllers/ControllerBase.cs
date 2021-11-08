using Microsoft.AspNetCore.Mvc;

namespace MassTransitTests.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
    }
}
