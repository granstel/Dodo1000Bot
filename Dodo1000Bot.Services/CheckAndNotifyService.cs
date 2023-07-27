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
        /// Check the <paramref name="value"/> is 0
        /// </summary>
        /// <param name="value">Any value</param>
        /// <returns><see langword="true"/> when the <paramref name="value"/> is 0</returns>
        protected bool CheckEquals0(int value)
        {
            return value == 0;
        }

        /// <summary>
        /// Check the value is greater than or equal to 1000
        /// </summary>
        /// <param name="value">Any value</param>
        /// <returns><see langword="true"/> when the <paramref name="value"/> is greater than or equal to 1000</returns>
        protected bool CheckGreaterOrEqual1000(int value)
        {
            return CheckGreaterOrEqualGivenValue(value, 1000);
        }

        /// <summary>
        /// Check the value is greater than or equal to the given value
        /// </summary>
        /// <param name="value">Any value</param>
        /// <param name="given">Given value</param>
        /// <returns><see langword="true"/> when the <paramref name="value"/> is greater than or equal to <paramref name="given"/> value</returns>
        protected bool CheckGreaterOrEqualGivenValue(decimal value, int given)
        {
            return value >= given;
        }
    }
}