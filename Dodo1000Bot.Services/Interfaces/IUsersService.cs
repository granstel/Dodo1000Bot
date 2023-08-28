using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public interface IUsersService
{
    Task Save(User user, CancellationToken cancellationToken);
    Task Delete(User user, CancellationToken cancellationToken);
}