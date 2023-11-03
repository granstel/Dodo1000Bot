using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public interface INotificationTemplatesRepository
    {
        Task<NotificationTemplate> GetRandom(NotificationType notificationType, Source messengerType, string languageCode, CancellationToken cancellationToken);
    }
}