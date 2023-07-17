using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class NotificationsService : INotificationsService
{
    private readonly ILogger<NotificationsService> _logger;
    private readonly INotificationsRepository _notificationsRepository;

    public NotificationsService(ILogger<NotificationsService> logger, INotificationsRepository notificationsRepository)
    {
        _logger = logger;
        _notificationsRepository = notificationsRepository;
    }

    public async Task Save(Event @event, CancellationToken cancellationToken)
    {
        var isExists = await _notificationsRepository.IsExists(@event, cancellationToken);

        if (isExists)
        {
            _logger.LogInformation("Notification with {fieldName}='{fieldValue}' is exists", 
                nameof(@event.Distinction), @event.Distinction);

            return;
        }

        await _notificationsRepository.Save(@event, cancellationToken);
    }

    public async Task PushNotifications(IEnumerable<INotifyService> notifyServices, CancellationToken cancellationToken)
    {
        IList<Event> notifications = await _notificationsRepository.GetNotPushedNotifications(cancellationToken);

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