using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public interface IMessengerService<in TInput, TOutput>
    {
        Task<TOutput> ProcessIncomingAsync(TInput input, CancellationToken cancellationToken);

        Task<bool> SetWebhookAsync(string url, CancellationToken cancellationToken);

        Task<bool> DeleteWebhookAsync(CancellationToken cancellationToken);
    }
}