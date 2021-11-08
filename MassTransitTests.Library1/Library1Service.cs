using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Commands;
using MassTransitTests.Shared;

namespace MassTransitTests.Library1
{
    public class Library1Service : GlobalService, ILibrary1Service
    {
        public Library1Service(IBus bus)
            : base(bus)
        {
        }

        public Task SendTestCommand()
        {
            return Publish(new TestCommand());
        }
    }
}