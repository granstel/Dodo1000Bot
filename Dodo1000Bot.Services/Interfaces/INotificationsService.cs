using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;

namespace Dodo1000Bot.Services;

public interface INotificationsService
{
    Task Save(Notification notification, CancellationToken cancellationToken);
}