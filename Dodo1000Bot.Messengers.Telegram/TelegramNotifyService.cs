using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;

namespace Dodo1000Bot.Messengers.Telegram;

public class TelegramNotifyService: INotifyService
{
    private readonly IUsersRepository _usersRepository;

    public TelegramNotifyService(UsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<IEnumerable<PushedNotification>> NotifyAbout(IEnumerable<Notification> notifications, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await _usersRepository.GetUsers(Source.Telegram, cancellationToken);

        var pushedNotifications = new List<PushedNotification>();

        foreach (var notification in notifications)
        {
            foreach (var user in users)
            {
                //TODO: magic

                pushedNotifications.Add(new PushedNotification
                {
                    NotificationId = notification.Id,
                    UserId = user.Id
                });
            }
        }

        return pushedNotifications;
    }
}