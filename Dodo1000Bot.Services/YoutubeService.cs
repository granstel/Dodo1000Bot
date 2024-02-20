using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.Youtube;
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

    public YoutubeService(
        ILogger<YoutubeService> log, 
        YoutubeConfiguration configuration, 
        IYouTubeClient youTubeClient, 
        INotificationsService notificationsService, 
        ISnapshotsRepository snapshotsRepository)
    {
        _log = log;
        _configuration = configuration;
        _youTubeClient = youTubeClient;
        _notificationsService = notificationsService;
        _snapshotsRepository = snapshotsRepository;
    }

    public async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        foreach(var channel in _configuration.Channels)
        {
            var snapshotName = GetChannelSnapshotName(channel);
            var videosSnapshot = 
                await _snapshotsRepository.Get<Video[]>(snapshotName, cancellationToken);
            var videos = await _youTubeClient.SearchVideos(channel, cancellationToken);
            
            var difference = videos.ExceptBy(videosSnapshot.Data.Select(v => v.Id), c => c.Id);

            foreach (var video in difference)
            {
                var notification = new Notification(NotificationType.Admin)
                {
                    Payload = new NotificationPayload
                    {
                        Text = $"Let's watch it together! https://youtu.be/{video.Id.VideoId} 👀"
                    }
                };

                await _notificationsService.Save(notification, cancellationToken);
            }
        }
    }

    private string GetChannelSnapshotName(string channel)
    {
        return $"{nameof(_youTubeClient.SearchVideos)}_{channel}";
    }
}