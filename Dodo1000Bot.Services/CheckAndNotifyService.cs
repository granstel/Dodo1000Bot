using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public abstract class CheckAndNotifyService
    {
        public abstract Task CheckAndNotify(CancellationToken cancellationToken);

        protected bool CheckThe1000Rule(int value)
        {
            return value > 0 && value % 1000 == 0;
        }

        protected bool CheckThe0Rule(int value)
        {
            return value == 0;
        }

        protected bool CheckThe1000Rule(decimal value)
        {
            return value > 0 && value % 1000 == 0;
        }
    }
}