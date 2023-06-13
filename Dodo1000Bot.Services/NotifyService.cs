using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;

namespace Dodo1000Bot.Services;

public class NotifyService : INotifyService
{
    private readonly INotificationsRepository _notificationsRepository;

    public NotifyService(INotificationsRepository notificationsRepository)
    {
        _notificationsRepository = notificationsRepository;
    }

    public Task Notify(Notification notification, CancellationToken cancellationToken)
    {
        return _notificationsRepository.Save(notification, cancellationToken);
    }
}