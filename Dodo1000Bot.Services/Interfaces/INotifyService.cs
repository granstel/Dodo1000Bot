using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services;

public interface INotifyService
{
    Task Notify(string notification, CancellationToken cancellationToken);
}