using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Models.PublicApi;

namespace Dodo1000Bot.Services;

using AllUnitsDictionary = Dictionary<Brands, Dictionary<UnitCountModel, IEnumerable<UnitInfo>>>;

public interface ISnapshotsService
{
    Task CreateAllUnitsSnapshotIfNotExists(CancellationToken cancellationToken);
    Task<IEnumerable<UnitInfo>> GetUnitInfoOfBrandAtCountrySnapshot(Brands brand, int countryId, CancellationToken cancellationToken);
    Task UpdateAllUnitsSnapshot(AllUnitsDictionary allUnits, CancellationToken cancellationToken);
}