using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Commands;
using MassTransitTests.DataTransferObjects.Responses;

namespace MassTransitTests.Library1.Consumers
{
    public class TestCommandConsumer : IConsumer<TestCommand>
    {
        public async Task Consume(ConsumeContext<TestCommand> context)
        {
            await context.RespondAsync(new TestCommandResponse());
        }
    }
}