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
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Dodo1000Bot.Messengers.Telegram;

public class TelegramNotifyService: INotifyService
{
    private readonly IUsersService _usersService;
    private readonly ITelegramBotClient _client;
    private readonly ILogger<TelegramNotifyService> _log;

    public TelegramNotifyService(IUsersService usersService, ITelegramBotClient client, ILogger<TelegramNotifyService> log)
    {
        _usersService = usersService;
        _client = client;
        _log = log;
    }

    public async Task<IEnumerable<PushedNotification>> NotifyAbout(IList<Notification> notifications,
        CancellationToken cancellationToken)
    {
        var pushedNotifications = new List<PushedNotification>();

        if (notifications?.Any() != true)
        {
            return pushedNotifications;
        }

        IEnumerable<User> users = await _usersService.GetUsers(Source.Telegram, cancellationToken);

        if (users?.Any() != true)
        {
            return pushedNotifications;
        }

        foreach (var user in users)
        {
            var pushedNotificationsToUsers = await PushNotificationsToUser(notifications, user, cancellationToken);

            pushedNotifications.AddRange(pushedNotificationsToUsers);
        }

        return pushedNotifications;
    }

    private async Task<IList<PushedNotification>> PushNotificationsToUser(IList<Notification> notifications, User user, CancellationToken cancellationToken)
    {
        var pushedNotifications = new List<PushedNotification>();

        foreach (var notification in notifications)
        {
            var messengerUserId = user.MessengerUserId;
            try
            {
                await _client.SendTextMessageAsync(messengerUserId, notification.Payload.Text,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);

                var coordinates = notification.Payload.Coordinates;
                if (coordinates != null)
                {
                    await _client.SendLocationAsync(messengerUserId, coordinates.Latitude, coordinates.Longitude,
                        cancellationToken: cancellationToken);
                }
            }
            catch (ApiRequestException e) when (e.ErrorCode == 403)
            {
                _log.LogWarning(e, "Error while send notification to {MessengerUserId}, so user will be deleted", messengerUserId);
                await _usersService.Delete(user, cancellationToken);
                return pushedNotifications;
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error while send notification to {MessengerUserId}", messengerUserId);
                return pushedNotifications;
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