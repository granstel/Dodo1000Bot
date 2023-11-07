using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Dodo1000Bot.Messengers.Telegram;

public class TelegramNotifyService : INotifyService
{
    private readonly ILogger<TelegramNotifyService> _log;
    private readonly ITelegramBotClient _client;
    private readonly IUsersRepository _usersRepository;
    private readonly INotificationTemplatesRepository _notificationTemplatesRepository;

    public Source Source => Source.Telegram;

    public TelegramNotifyService(
        ILogger<TelegramNotifyService> log,
        ITelegramBotClient client,
        IUsersRepository usersRepository,
        INotificationTemplatesRepository notificationTemplatesRepository
        )
    {
        _log = log;
        _client = client;
        _usersRepository = usersRepository;
        _notificationTemplatesRepository = notificationTemplatesRepository;
    }

    public async Task<IEnumerable<PushedNotification>> NotifyAbout(IList<Notification> notifications,
        CancellationToken cancellationToken)
    {
        var pushedNotifications = new List<PushedNotification>();

        if (notifications?.Any() != true)
        {
            return pushedNotifications;
        }

        IList<User> users = await _usersRepository.GetUsers(Source, cancellationToken);

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

        const string defaultLanguageCode = "en";

        foreach (var notification in notifications)
        {
            var template = await _notificationTemplatesRepository.GetRandom(notification.Type, Source, user.LanguageCode ?? defaultLanguageCode, cancellationToken);

            var text = notification.Payload.Text;

            if (template?.Template is not null)
            {
                text = template.Template;
            }

            var properties = notification.Payload.TemplateArguments.Deserialize<JObject>();
            foreach (var property in properties)
            {
                var name = property.Key;
                var value = property.Value.Value<string>();
                text = text.Replace($"{{{name}}}", value);
            }

            var messengerUserId = user.MessengerUserId;
            try
            {
                var coordinates = notification.Payload.Coordinates;
                if (coordinates != null)
                {
                    await _client.SendLocationAsync(messengerUserId, coordinates.Latitude, coordinates.Longitude,
                        cancellationToken: cancellationToken);
                }
                await _client.SendTextMessageAsync(messengerUserId, text, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
            }
            catch (ApiRequestException e) when (e.ErrorCode == 403)
            {
                _log.LogWarning(e, "Error while send notification to {MessengerUserId}, so user will be deleted", messengerUserId);
                await _usersRepository.Delete(user, cancellationToken);
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

    public async Task SendToAdmin(Notification notification, CancellationToken cancellationToken)
    {
        var admins = await _usersRepository.GetAdmins(Source.Telegram, cancellationToken);

        foreach (var admin in admins)
        {
            try
            {
                await _client.SendTextMessageAsync(admin.MessengerUserId, notification.Payload.Text, parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error while send notification to {MessengerUserId}", admin.MessengerUserId);
            }
        }
    }
}