using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public abstract class CheckAndNotifyService
    {
        public abstract Task CheckAndNotify(CancellationToken cancellationToken);

        protected bool CheckTheRule(int value)
        {
            return value % 1000 == 0;
        }

        protected bool CheckTheRule(decimal value)
        {
            return value % 1000 == 0;
        }
    }
}