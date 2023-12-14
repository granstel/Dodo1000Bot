using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public abstract class CheckAndNotifyService : IAsyncDisposable
    {
        public abstract Task CheckAndNotify(CancellationToken cancellationToken);

        protected abstract ValueTask DisposeAsyncCore();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }
    }
}