using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;

namespace Dodo1000Bot.Services;

public interface INotifyService
{
    public Task NotifyAbout(Notification notification, CancellationToken cancellationToken);

    Task NotifyAbout(IEnumerable<Notification> notifications, CancellationToken cancellationToken);
}