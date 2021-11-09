using System.Threading.Tasks;
using MassTransit;
using MassTransit.Mediator;
using MassTransitTests.DataTransferObjects.Commands;
using MassTransitTests.DataTransferObjects.Events;
using MassTransitTests.DataTransferObjects.Queries;
using MassTransitTests.Library1;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitTests.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly ILibrary1Service _library1Service;
        private readonly IMediator _mediator;

        public TestsController(IBus bus, ILibrary1Service library1Service)
        {
            _bus = bus;
            _library1Service = library1Service;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendRequest()
        {
            await _bus.Publish(new GetDataQuery());

            return Ok();
        }

        [HttpPost("event")]
        public async Task<IActionResult> SendEvent()
        {
            await _bus.Publish(new TranslationChangedEvent());

            return Ok();
        }

        [HttpPost("command")]
        public async Task<IActionResult> SendCommand()
        {
            await _bus.Publish(new SendToOneOfManyCommand());

            return Ok();
        }

        [HttpPost("mediator")]
        public async Task<IActionResult> SendMediator()
        {
            await _mediator.Send(new GetMediatorResponseQuery());

            return Ok();
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestCommand()
        {
            await _library1Service.SendTestCommand();
            
            return Ok();
        }
    }
}
