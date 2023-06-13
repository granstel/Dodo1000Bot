using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;

namespace Dodo1000Bot.Services;

public interface INotifyService
{
    Task Notify(Notification notification, CancellationToken cancellationToken);
}