using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public abstract class CheckAndNotifyService
    {
        public abstract Task CheckAndNotify(CancellationToken cancellationToken);
    }
}