using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public class NotificationsService : INotificationsService
{
    private readonly INotificationsRepository _notificationsRepository;

    public NotificationsService(INotificationsRepository notificationsRepository)
    {
        _notificationsRepository = notificationsRepository;
    }

    public Task Save(Notification notification, CancellationToken cancellationToken)
    {
        return _notificationsRepository.Save(notification, cancellationToken);
    }

    public async Task PushNotifications(IEnumerable<INotifyService> notifyServices, CancellationToken cancellationToken)
    {
        IList<Notification> notifications = await _notificationsRepository.GetNotPushedNotifications(cancellationToken);

        if (!notifications.Any())
        {
            return;
        }

        IEnumerable<Task<IEnumerable<PushedNotification>>> tasks = notifyServices.Select(s => s.NotifyAbout(notifications, cancellationToken));

        IEnumerable<PushedNotification>[] tasksResults = await Task.WhenAll(tasks);

        IEnumerable<PushedNotification> pushedNotifications = tasksResults.SelectMany(r => r);

        await _notificationsRepository.Save(pushedNotifications, cancellationToken);
    }
}