using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services;

public interface ISnapshotsService
{
    Task CreateUnitsCountSnapshotIfNotExists(CancellationToken cancellationToken);
    Task CreateAllUnitsSnapshotIfNotExists(CancellationToken cancellationToken);
    Task CreateUnitsSnapshotIfNotExists(CancellationToken cancellationToken);
}