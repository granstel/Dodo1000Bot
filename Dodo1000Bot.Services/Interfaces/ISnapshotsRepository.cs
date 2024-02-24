using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;

namespace Dodo1000Bot.Services
{
    public interface ISnapshotsRepository
    {
        Task<Snapshot<TData>> Get<TData>(string snapshotName, CancellationToken cancellationToken);

        Task Save<TData>(Snapshot<TData> snapshot, CancellationToken cancellationToken);
    }
}