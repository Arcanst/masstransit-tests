using System.Threading.Tasks;
using MassTransit;
using MassTransitTests.DataTransferObjects.Commands;
using MassTransitTests.DataTransferObjects.Responses;
using MassTransitTests.Shared;

namespace MassTransitTests.Library1
{
    public class Library1Service : GlobalService, ILibrary1Service
    {
        public Library1Service(IBus bus)
            : base(bus)
        {
        }

        public Task<TestResponse> SendTestCommand()
        {
            return Request<TestCommand, TestResponse>(new TestCommand());
        }
    }
}