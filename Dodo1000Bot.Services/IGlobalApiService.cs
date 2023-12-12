using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.GlobalApi;

namespace Dodo1000Bot.Services;

public interface IGlobalApiService
{
    Task CreateUnitsCountSnapshotIfNotExists(CancellationToken cancellationToken);
    Task<BrandListTotalUnitCountListModel> GetUnitsCount(CancellationToken cancellationToken);
    Task<BrandListTotalUnitCountListModel> GetUnitsCountSnapshot(CancellationToken cancellationToken);
    Task UpdateUnitsCountSnapshot(CancellationToken cancellationToken);
    Task<IEnumerable<Brand>> GetBrands(CancellationToken cancellationToken);
}