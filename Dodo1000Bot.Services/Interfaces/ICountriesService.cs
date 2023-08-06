using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services;

public interface ICountriesService
{
    Task<string> GetName(string code, CancellationToken cancellationToken);
}