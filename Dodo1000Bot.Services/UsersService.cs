using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services.Metrics;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Dodo1000Bot.Services.Tests")]

namespace Dodo1000Bot.Services;

public class UsersService : IUsersService
{
    private readonly ILogger<UsersService> _log;
    private readonly IUsersRepository _usersRepository;
    private readonly ChannelWriter<Notification> _notificationsChannel;

    public UsersService(
        ILogger<UsersService> log, 
        IUsersRepository usersRepository, 
        ChannelWriter<Notification> notificationsChannel)
    {
        _log = log;
        _usersRepository = usersRepository;
        _notificationsChannel = notificationsChannel;
    }

    public async Task SaveAndNotify(User user, CancellationToken cancellationToken)
    {
        await _usersRepository.SaveUser(user, cancellationToken);

        await CheckAndNotifyAboutSubscribers(cancellationToken);
    }

    internal async Task CheckAndNotifyAboutSubscribers(CancellationToken cancellationToken)
    {
        try
        {
            var subscribersCount = await _usersRepository.Count(cancellationToken);

            if (!CheckHelper.CheckRemainder(subscribersCount, 100))
            {
                return;
            }

            var notification = new Notification(NotificationType.SubscribersCount)
            {
                Payload = new NotificationPayload
                {
                    Text = $"👥 Hey! I already have {subscribersCount} subscribers! Thank you for staying with me 🤗",
                }
            };

            await _notificationsChannel.WriteAsync(notification, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't send notification about users count");
        }
    }

    public async Task Delete(User user, CancellationToken cancellationToken)
    {
        await _usersRepository.Delete(user, cancellationToken);
        await Count(cancellationToken);
    }

    public Task<IList<User>> GetUsers(Source messengerType, CancellationToken cancellationToken) =>
        _usersRepository.GetUsers(messengerType, cancellationToken);

    public Task<IList<User>> GetAdmins(Source messengerType, CancellationToken cancellationToken) =>
        _usersRepository.GetAdmins(messengerType, cancellationToken);

    public async Task Count(CancellationToken cancellationToken)
    {
        var count = await _usersRepository.Count(cancellationToken);
        MetricsCollector.Set("users", count);
    }
}
