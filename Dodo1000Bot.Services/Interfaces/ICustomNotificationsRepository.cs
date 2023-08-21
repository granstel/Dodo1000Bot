using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface ICustomNotificationsRepository
{
    Task Save(Notification notification, CancellationToken cancellationToken);
    Task<Notification> Get(int id, CancellationToken cancellationToken);
    Task Delete(int id, CancellationToken cancellationToken);
}