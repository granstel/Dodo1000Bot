using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class NotificationsService : INotificationsService
{
    private readonly ILogger<NotificationsService> _logger;
    private readonly IEnumerable<INotifyService> _notifyServices;
    private readonly INotificationsRepository _notificationsRepository;

    public NotificationsService(ILogger<NotificationsService> logger, IEnumerable<INotifyService> notifyServices, 
        INotificationsRepository notificationsRepository)
    {
        _logger = logger;
        _notifyServices = notifyServices;
        _notificationsRepository = notificationsRepository;
    }

    public async Task Save(Notification notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification?.Payload?.Text))
        {
            _logger.LogWarning("Text of notification payload is null or empty");
            return;
        }

        await _notificationsRepository.Save(notification, cancellationToken);
    }

    public async Task PushNotifications(CancellationToken cancellationToken)
    {
        IList<Notification> notifications = await _notificationsRepository.GetNotPushedNotifications(cancellationToken);

        if (!notifications.Any())
        {
            return;
        }

        var sortedNotifications = notifications.Where(n => n.Type != NotificationType.Admin).ToList();

        IEnumerable<Task<IEnumerable<PushedNotification>>> tasks = _notifyServices.Select(s => s.NotifyAbout(sortedNotifications, cancellationToken));

        IEnumerable<PushedNotification>[] tasksResults = await Task.WhenAll(tasks);

        IEnumerable<PushedNotification> pushedNotifications = tasksResults.SelectMany(r => r);

        await _notificationsRepository.Save(pushedNotifications, cancellationToken);
    }

    public async Task Delete(int notificationId, CancellationToken cancellationToken)
    {
        await _notificationsRepository.Delete(notificationId, cancellationToken);
    }

    public async Task SendToAdmin(Notification notification, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Task> tasks = 
                _notifyServices.Select(s => s.SendToAdmin(notification, cancellationToken));

            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Can't send to admin");
        }
    }
}