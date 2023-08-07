using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services;

public interface ICountriesRepository
{
    Task<string> GetName(string code, CancellationToken cancellationToken);

    Task Save(string code, string name, CancellationToken cancellationToken1);
}