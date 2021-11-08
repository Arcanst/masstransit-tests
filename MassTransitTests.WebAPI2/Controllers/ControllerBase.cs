using Microsoft.AspNetCore.Mvc;

namespace MassTransitTests.WebAPI2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
    }
}
