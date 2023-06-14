﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
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
        IEnumerable<Notification> notifications = await _notificationsRepository.GetNotPushedNotifications(cancellationToken);

        var tasks = notifyServices.Select(s => s.NotifyAbout(notifications, cancellationToken));

        IEnumerable<PushedNotification> pushedNotifications = await Task.WhenAll(tasks);

        await _notificationsRepository.Save(pushedNotifications, cancellationToken);
    }
}