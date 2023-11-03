using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface INotifyService
{
    Source Source { get; }

    Task<IEnumerable<PushedNotification>> NotifyAbout(IList<Notification> notifications,
        CancellationToken cancellationToken);

    Task SendToAdmin(Notification notification, CancellationToken cancellationToken);
}