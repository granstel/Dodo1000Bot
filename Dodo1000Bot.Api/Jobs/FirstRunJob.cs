using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Hosting;

namespace Dodo1000Bot.Api.Jobs;

public class FirstRunJob: IHostedService
{
    private readonly UnitsService _unitsService;
    private readonly YoutubeService _youtubeService;

    public FirstRunJob(UnitsService unitsService, YoutubeService youtubeService)
    {
        _unitsService = unitsService;
        _youtubeService = youtubeService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _unitsService.CreateUnitsCountSnapshotIfNotExists(cancellationToken);
        await _unitsService.CreateUnitsSnapshotIfNotExists(cancellationToken);
        
        await _youtubeService.CreateVideosSnapshotIfNotExists(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}