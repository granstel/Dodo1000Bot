using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task Save(User user, CancellationToken cancellationToken)
    {
        await _usersRepository.SaveUser(user, cancellationToken);
    }

    public async Task Delete(User user, CancellationToken cancellationToken)
    {
        await _usersRepository.Delete(user, cancellationToken);
    }
}