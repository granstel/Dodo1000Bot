﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Dodo1000Bot.Messengers.Telegram;

public class TelegramNotifyService : INotifyService
{
    private readonly ILogger<TelegramNotifyService> _log;
    private readonly IUsersRepository _usersRepository;
    private readonly ITelegramBotClient _client;

    public TelegramNotifyService(ILogger<TelegramNotifyService> log, IUsersRepository usersRepository, ITelegramBotClient client)
    {
        _usersRepository = usersRepository;
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

        IEnumerable<User> users = await _usersRepository.GetUsers(Source.Telegram, cancellationToken);

        if (users?.Any() != true)
        {
            return pushedNotifications;
        }

        foreach (var user in users)
        {
            var pushedNotificationsToUsers = await PushNotifications(notifications, user, cancellationToken);

            pushedNotifications.AddRange(pushedNotificationsToUsers);
        }

        return pushedNotifications;
    }

    private async Task<IList<PushedNotification>> PushNotifications(
        IList<Notification> notifications,
        User user,
        CancellationToken cancellationToken)
    {
        var pushedNotifications = new List<PushedNotification>();

        foreach (var notification in notifications)
        {
            var messengerUserId = user.MessengerUserId;
            try
            {
                await _client.SendTextMessageAsync(messengerUserId, notification.Payload.Text, parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);

                var coordinates = notification.Payload.Coordinates;
                if (coordinates != null)
                {
                    await _client.SendLocationAsync(messengerUserId, coordinates.Latitude, coordinates.Longitude, 
                        cancellationToken: cancellationToken);
                }
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