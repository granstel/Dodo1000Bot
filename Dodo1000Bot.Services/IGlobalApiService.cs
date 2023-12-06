using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.GlobalApi;

namespace Dodo1000Bot.Services;

public interface IGlobalApiService
{
    Task CreateUnitsCountSnapshotIfNotExists(CancellationToken cancellationToken);
    Task<BrandListTotalUnitCountListModel> GetUnitsCountAndUpdateSnapshot(CancellationToken cancellationToken);
    Task<BrandListTotalUnitCountListModel> GetUnitsCountSnapshot(CancellationToken cancellationToken);
}