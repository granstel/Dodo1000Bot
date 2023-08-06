using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Restcountries;

namespace Dodo1000Bot.Services;

public interface IRestcountriesApiClient
{
    Task<Country[]> GetNames(CancellationToken cancellationToken, params string[] codes);
}