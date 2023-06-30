using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Dodo1000Bot.Messengers.Telegram;

public class TelegramNotifyService: INotifyService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ITelegramBotClient _client;
    private readonly ILogger<TelegramNotifyService> _log;

    public TelegramNotifyService(IUsersRepository usersRepository, ITelegramBotClient client, ILogger<TelegramNotifyService> log)
    {
        _usersRepository = usersRepository;
        _client = client;
        _log = log;
    }

    public async Task<IEnumerable<PushedNotification>> NotifyAbout(IEnumerable<Notification> notifications, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await _usersRepository.GetUsers(Source.Telegram, cancellationToken);

        var notificationsArray = notifications.ToArray();
        var usersArray = users.ToArray();

        if (!notificationsArray.Any() || !usersArray.Any())
        {
            return Enumerable.Empty<PushedNotification>();
        }

        var pushedNotifications = new List<PushedNotification>();

        foreach (var user in usersArray)
        {
            var pushedNotificationsToUsers = await PushNotificationsToUsers(notificationsArray, user, cancellationToken);

            pushedNotifications.AddRange(pushedNotificationsToUsers);
        }

        return pushedNotifications;
    }

    private async Task<IList<PushedNotification>> PushNotificationsToUsers(Notification[] notificationsArray, User user, CancellationToken cancellationToken)
    {
        var pushedNotifications = new List<PushedNotification>();

        foreach (var notification in notificationsArray)
        {
            try
            {
                await _client.SendTextMessageAsync(user.MessengerUserId, notification.Payload.Text,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error while send text message");
                return null;
            }

            var pushedNotification = new PushedNotification
            {
                NotificationId = notification.Id,
                UserId = user.Id
            };

            pushedNotifications.Add(pushedNotification);
        }

        return pushedNotifications;
    }
}