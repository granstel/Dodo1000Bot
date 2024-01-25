using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class SocialNetworksService: CheckAndNotifyService
{
    private readonly ILogger<SocialNetworksService> _log;
    private readonly IYouTubeClient _youTubeClient;
    private readonly INotificationsService _notificationsService;
    private readonly ISnapshotsRepository _snapshotsRepository;

    public SocialNetworksService(ILogger<SocialNetworksService> log)
    {
        _log = log;
    }

    public override Task CheckAndNotify(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}