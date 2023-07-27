using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public abstract class CheckAndNotifyService
    {
        public abstract Task CheckAndNotify(CancellationToken cancellationToken);

        /// <summary>
        /// Check remainder of value divided by 1000
        /// </summary>
        /// <param name="value">Any value</param>
        /// <returns><see langword="true"/> when the <paramref name="value"/> is over than 0 and remainder of division by 1000 equals 0</returns>
        protected bool CheckRemainder1000(int value)
        {
            return value > 0 && value % 1000 == 0;
        }

        /// <summary>
        /// Check remainder of value divided by 1000
        /// </summary>
        /// <param name="value">Any value</param>
        /// <returns><see langword="true"/> when the <paramref name="value"/> is over than 0 and remainder of division by 1000 equals 0</returns>
        protected bool CheckRemainder1000(decimal value)
        {
            return value > 0 && value % 1000 == 0;
        }

        /// <summary>
        /// Check is <paramref name="value"/> equals 0
        /// </summary>
        /// <param name="value">Any value</param>
        /// <returns><see langword="true"/> when the <paramref name="value"/> is 0</returns>
        protected bool CheckEquals0(int value)
        {
            return value == 0;
        }
    }
}