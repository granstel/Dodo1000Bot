using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public interface CheckAndNotifyService
    {
        public Task CheckAndNotify(CancellationToken cancellationToken);
    }
}