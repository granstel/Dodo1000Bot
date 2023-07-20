using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public interface IUsersRepository
    {
        Task<IList<User>> GetUsers(Source messengerType, CancellationToken cancellationToken);

        Task SaveUser(User user, CancellationToken ct);

        Task<bool> IsExists(User user, CancellationToken cancellationToken);
    }
}