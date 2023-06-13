using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services;

namespace Dodo1000Bot.Messengers.Telegram;

public class TelegramNotifyService: INotifyService
{
    public Task NotifyAbout(Notification notification, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task NotifyAbout(IEnumerable<Notification> notifications, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}