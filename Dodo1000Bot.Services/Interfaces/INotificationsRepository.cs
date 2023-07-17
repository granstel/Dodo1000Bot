using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface INotificationsRepository
{
    Task Save(Event @event, CancellationToken cancellationToken);

    Task<IList<Event>> GetNotPushedNotifications(CancellationToken cancellationToken);

    Task Save(IEnumerable<PushedNotification> pushedNotifications, CancellationToken cancellationToken);

    Task<bool> IsExists(Event @event, CancellationToken cancellationToken);
}