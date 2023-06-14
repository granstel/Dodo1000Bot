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

    public Task Save(NotificationPayload notificationPayload, CancellationToken cancellationToken)
    {
        return _notificationsRepository.Save(notificationPayload, cancellationToken);
    }

    public async Task PushNotifications(IEnumerable<INotifyService> notifyServices, CancellationToken cancellationToken)
    {
        IEnumerable<Notification> notifications = await _notificationsRepository.GetNotPushedNotifications(cancellationToken);

        var tasks = notifyServices.Select(s => s.NotifyAbout(notifications, cancellationToken));

        var tasksResults = await Task.WhenAll(tasks);

        var pushedNotifications = tasksResults.SelectMany(n => n);

        await _notificationsRepository.Save(pushedNotifications, cancellationToken);
    }
}