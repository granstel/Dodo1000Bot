using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Logging;

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

    public async Task Save(User user, CancellationToken cancellationToken)
    {
        await _usersRepository.SaveUser(user, cancellationToken);

        try
        {
            var usersCount = await _usersRepository.Count(cancellationToken);

            if (!CheckHelper.CheckRemainder(usersCount, 10))
            {
                return;
            }

            var notification = new Notification
            {
                Payload = new NotificationPayload
                {
                    Text = $"👥 Hey! I already have {usersCount} subscribers! Thank you for staying with me 🤗",
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't send notification about subscribers count");
        }
    }

    public async Task Delete(User user, CancellationToken cancellationToken)
    {
        await _usersRepository.Delete(user, cancellationToken);
    }
}