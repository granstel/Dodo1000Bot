using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public interface ICheckAndNotifyService
    {
        Task CheckAndNotify(CancellationToken cancellationToken);
    }
}