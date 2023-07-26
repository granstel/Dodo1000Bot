using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.RealtimeBoard;

namespace Dodo1000Bot.Services
{
    public interface IRealtimeBoardApiClient
    {
        Task<Statistics> Statistics(CancellationToken cancellationToken);
    }
}