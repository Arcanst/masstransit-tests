using System.Threading.Tasks;
using MassTransitTests.DataTransferObjects.Responses;

namespace MassTransitTests.Library1
{
    public interface ILibrary1Service
    {
        Task<TestResponse> SendTestCommand();
    }
}