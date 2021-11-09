using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects;

namespace MassTransitTests.Shared
{
    public abstract class GlobalService
    {
        private readonly IBus _bus;

        protected GlobalService(IBus bus)
        {
            _bus = bus;
        }

        protected Task Publish<TMessage>(TMessage message)
            where TMessage : class, IMessage
        {
            return _bus.Publish(message);
        }

        protected async Task<TResponse> Request<TRequest, TResponse>(TRequest request)
            where TRequest : class, IMessage
            where TResponse : class
        {
            var response = await _bus.Request<TRequest, TResponse>(request);

            return response.Message;
        }
    }
}