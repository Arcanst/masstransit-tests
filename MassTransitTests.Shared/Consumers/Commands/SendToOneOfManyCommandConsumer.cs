using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Commands;

namespace MassTransitTests.Shared.Consumers.Commands
{
    public class SendToOneOfManyCommandConsumer : IConsumer<SendToOneOfManyCommand>
    {
        public async Task Consume(ConsumeContext<SendToOneOfManyCommand> context)
        {
            Console.WriteLine($"{nameof(SendToOneOfManyCommandConsumer)} - {Assembly.GetEntryAssembly().FullName}");
            Thread.Sleep(10000);
        }
    }
}
