using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Models.PublicApi;

namespace Dodo1000Bot.Services;

using AllUnitsDictionary = Dictionary<Brands, Dictionary<UnitCountModel, IEnumerable<UnitInfo>>>;

public interface IPublicApiService
{
    Task CreateAllUnitsSnapshotIfNotExists(CancellationToken cancellationToken);
    Task<IEnumerable<UnitInfo>> GetUnitInfoOfBrandAtCountrySnapshot(Brands brand, string countryCode,
        CancellationToken cancellationToken);
    Task UpdateAllUnitsSnapshot(Dictionary<Brands, Dictionary<UnitCountModel, IEnumerable<UnitInfo>>> allUnits, CancellationToken cancellationToken);
    Task<AllUnitsDictionary> GetAllUnits(IList<BrandTotalUnitCountListModel> brands, CancellationToken cancellationToken);
}