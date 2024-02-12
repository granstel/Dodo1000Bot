using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services.Configuration;
using Dodo1000Bot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class YoutubeService: CheckAndNotifyService
{
    private readonly ILogger<YoutubeService> _log;
    private readonly YoutubeConfiguration _configuration;
    private readonly IYouTubeClient _youTubeClient;
    private readonly INotificationsService _notificationsService;
    private readonly ISnapshotsRepository _snapshotsRepository;

    public YoutubeService(ILogger<YoutubeService> log, YoutubeConfiguration configuration)
    {
        _log = log;
        _configuration = configuration;
    }

    public async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        foreach(var channel in _configuration.Channels)
        {
            var videos = await _youTubeClient.Videos(channel, cancellationToken);
        }
    }
}