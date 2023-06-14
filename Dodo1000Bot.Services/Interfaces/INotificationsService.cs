using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface INotificationsService
{
    Task Save(Notification notification, CancellationToken cancellationToken);

    Task PushNotifications(IEnumerable<INotifyService> notifyServices, CancellationToken cancellationToken);
}