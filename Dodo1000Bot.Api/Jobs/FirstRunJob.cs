using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Hosting;

namespace Dodo1000Bot.Api.Jobs;

public class FirstRunJob: IHostedService
{
    private readonly UnitsService _unitsService;
    private readonly YoutubeService _youtubeService;
    private readonly IUsersService _usersService;

    public FirstRunJob(
        UnitsService unitsService, 
        YoutubeService youtubeService,
        IUsersService usersService)
    {
        _unitsService = unitsService;
        _youtubeService = youtubeService;
        _usersService = usersService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _unitsService.CreateUnitsCountSnapshotIfNotExists(cancellationToken);
        await _unitsService.CreateUnitsSnapshotIfNotExists(cancellationToken);
        
        await _youtubeService.CreateVideosSnapshotIfNotExists(cancellationToken);

        await _usersService.Count(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}