using System;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Queries;
using MassTransitTests.DataTransferObjects.Responses;

namespace MassTransitTests.Shared.Consumers.Queries
{
    public class GetDataQueryConsumer : IConsumer<GetDataQuery>
    {
        public async Task Consume(ConsumeContext<GetDataQuery> context)
        {
            Console.WriteLine($"{nameof(GetDataQueryConsumer)} - {Assembly.GetEntryAssembly().FullName}");
            await context.RespondAsync(new GetDataResponse());
        }
    }
}
