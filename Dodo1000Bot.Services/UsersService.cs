using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Dodo1000Bot.Services.Tests")]

namespace Dodo1000Bot.Services;

public class UsersService : IUsersService
{
    private readonly ILogger<UsersService> _log;
    private readonly IUsersRepository _usersRepository;
    private readonly INotificationsService _notificationsService;

    public UsersService(
        ILogger<UsersService> log, 
        IUsersRepository usersRepository, 
        INotificationsService notificationsService)
    {
        _log = log;
        _usersRepository = usersRepository;
        _notificationsService = notificationsService;
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

            var notification = new Notification
            {
                Payload = new NotificationPayload
                {
                    Text = $"👥 Hey! I already have {subscribersCount} subscribers! Thank you for staying with me 🤗",
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't send notification about users count");
        }
    }

    public async Task Delete(User user, CancellationToken cancellationToken)
    {
        await _usersRepository.Delete(user, cancellationToken);
    }
}
