using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface INotificationsRepository
{
    Task Save(Notification notification, CancellationToken cancellationToken);

    Task<IEnumerable<Notification>> GetNotPushedNotifications(CancellationToken cancellationToken);

    Task Save(IEnumerable<PushedNotification> pushedNotifications, CancellationToken cancellationToken);
}