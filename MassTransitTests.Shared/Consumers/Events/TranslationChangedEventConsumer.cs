using System;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Events;

namespace MassTransitTests.Shared.Consumers.Events
{
    public class TranslationChangedEventConsumer : IConsumer<TranslationChangedEvent>
    {
        public async Task Consume(ConsumeContext<TranslationChangedEvent> context)
        {
            Console.WriteLine($"{nameof(TranslationChangedEventConsumer)} - {Assembly.GetEntryAssembly().FullName}");
        }
    }
}
