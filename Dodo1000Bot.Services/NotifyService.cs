using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services;

public class NotifyService : INotifyService
{
    public Task Notify(string notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}