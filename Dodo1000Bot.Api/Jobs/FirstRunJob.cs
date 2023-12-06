using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dodo1000Bot.Api.Jobs;

public class FirstRunJob: IHostedService
{
    private readonly IServiceProvider _provider;

    public FirstRunJob(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var snapshotsService = scope.ServiceProvider.GetRequiredService<ISnapshotsService>();

        await snapshotsService.CreateUnitsCountSnapshotIfNotExists(cancellationToken);
        await snapshotsService.CreateUnitsSnapshotIfNotExists(cancellationToken);
        await snapshotsService.CreateAllUnitsSnapshotIfNotExists(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}