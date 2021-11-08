using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Commands;

namespace MassTransitTests.Library1.Consumers
{
    public class TestCommandConsumer : IConsumer<TestCommand>
    {
        public async Task Consume(ConsumeContext<TestCommand> context)
        {
        }
    }
}