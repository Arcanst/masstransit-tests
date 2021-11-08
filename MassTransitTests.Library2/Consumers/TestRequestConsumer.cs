using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Queries;

namespace MassTransitTests.Library2.Consumers
{
    public class TestRequestConsumer : IConsumer<TestQuery>
    {
        public async Task Consume(ConsumeContext<TestQuery> context)
        {
        }
    }
}