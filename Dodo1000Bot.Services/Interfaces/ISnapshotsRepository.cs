using Dodo1000Bot.Models.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services.Interfaces
{
    public interface ISnapshotsRepository
    {
        Task Save<TData>(Snapshot<TData> snapshot, CancellationToken cancellationToken);
    }
}