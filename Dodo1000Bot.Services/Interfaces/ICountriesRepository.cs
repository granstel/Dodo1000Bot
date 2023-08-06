using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.GlobalApi;

namespace Dodo1000Bot.Services;

public interface ICountriesRepository
{
    Task<string> GetName(int id, CancellationToken cancellationToken);

    Task Save(UnitCountModel country, CancellationToken cancellationToken);
}