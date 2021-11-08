using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Queries;

namespace MassTransitTests.Shared.Consumers.Queries
{
    public class GetMediatorResponseQueryConsumer : IConsumer<GetMediatorResponseQuery>
    {
        public async Task Consume(ConsumeContext<GetMediatorResponseQuery> context)
        {
            Thread.Sleep(10000);
        }
    }
}
