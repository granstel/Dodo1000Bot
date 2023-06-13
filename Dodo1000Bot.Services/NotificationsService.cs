using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;

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
        var notifications = await _notificationsRepository.GetNotPushedNotifications(cancellationToken);

        var tasks = notifyServices.Select(s => s.NotifyAbout(notifications, cancellationToken));

        await Task.WhenAll(tasks);
    }
}