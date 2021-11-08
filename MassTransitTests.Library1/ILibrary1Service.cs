using System.Threading.Tasks;

namespace MassTransitTests.Library1
{
    public interface ILibrary1Service
    {
        Task SendTestCommand();
    }
}