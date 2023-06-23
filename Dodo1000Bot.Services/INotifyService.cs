using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface INotifyService
{
    Task<IEnumerable<PushedNotification>> NotifyAbout(IEnumerable<Notification> notifications, CancellationToken cancellationToken);
}