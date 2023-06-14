using System;
using System.Collections.Generic;
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

    public TelegramNotifyService(UsersRepository usersRepository, ITelegramBotClient client, ILogger<TelegramNotifyService> log)
    {
        _usersRepository = usersRepository;
        _client = client;
        _log = log;
    }

    public async Task<IEnumerable<PushedNotification>> NotifyAbout(IEnumerable<Notification> notifications, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await _usersRepository.GetUsers(Source.Telegram, cancellationToken);

        var pushedNotifications = new List<PushedNotification>();

        foreach (var notification in notifications)
        {
            foreach (var user in users)
            {
                await SendTextMessageAsync(user.MessengerUserId, notification.Payload.Text);

                pushedNotifications.Add(new PushedNotification
                {
                    NotificationId = notification.Id,
                    UserId = user.Id
                });
            }
        }

        return pushedNotifications;
    }

    private async Task SendTextMessageAsync(string chatId, string text)
    {
        try
        {
            await _client.SendTextMessageAsync(chatId, text);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Error while send text message");
        }
    }
}