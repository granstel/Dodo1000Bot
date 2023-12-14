using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Models.PublicApi;

namespace Dodo1000Bot.Services;

using AllUnitsDictionary = Dictionary<string, Dictionary<Country, IEnumerable<UnitInfo>>>;

public interface IPublicApiService
{
    Task CreateAllUnitsSnapshotIfNotExists(CancellationToken cancellationToken);
    Task<IEnumerable<UnitInfo>> GetUnitInfoOfBrandAtCountrySnapshot(string brand, string countryCode,
        CancellationToken cancellationToken);
    Task UpdateAllUnitsSnapshot(CancellationToken cancellationToken);
    Task<AllUnitsDictionary> AllUnits(CancellationToken cancellationToken);
}