using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class YoutubeService: CheckAndNotifyService
{
    private readonly ILogger<YoutubeService> _log;
    private readonly IYouTubeClient _youTubeClient;
    private readonly INotificationsService _notificationsService;
    private readonly ISnapshotsRepository _snapshotsRepository;

    public YoutubeService(ILogger<YoutubeService> log)
    {
        _log = log;
    }

    public override Task CheckAndNotify(CancellationToken cancellationToken)
    {
        var videos = await _youTubeClient.Videos(cancellationToken);
    }
}