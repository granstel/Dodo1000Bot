using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface INotificationsService
{
    Task Save(Notification notification, CancellationToken cancellationToken);

    Task PushNotifications(CancellationToken cancellationToken);

    Task Delete(int notificationId, CancellationToken cancellationToken);
}