using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public interface IMessengerService<TInput, TOutput>
    {
        Task<TOutput> ProcessIncomingAsync(TInput input);

        Task<bool> SetWebhookAsync(string url);

        Task<bool> DeleteWebhookAsync();
    }
}