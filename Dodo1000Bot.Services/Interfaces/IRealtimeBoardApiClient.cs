using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public interface IRealtimeBoardApiClient
    {
        Task<int> OrdersPerMinute(CancellationToken cancellationToken);
    }
}