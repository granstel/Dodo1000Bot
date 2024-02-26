using System;
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
    private static readonly string[] LiveBroadcastContentValidValues = {
        "live"
    };

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
            
            var difference = videos.ExceptBy(videosSnapshot.Data.Select(v => v.Id.VideoId), c => c.Id.VideoId);

            foreach (var video in difference)
            {
                if (!LiveBroadcastContentValidValues.Contains(video.Snippet.LiveBroadcastContent))
                {
                    continue;
                }

                var notification = new Notification(NotificationType.Admin)
                {
                    Payload = new NotificationPayload
                    {
                        Text = $"Let's watch it together! https://youtu.be/{video.Id.VideoId} 👀"
                    }
                };

                await _notificationsService.Save(notification, cancellationToken);
            }

            await UpdateSnapshot(snapshotName, videos, cancellationToken);
        }
    }

    public async Task CreateVideosSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        foreach (var channel in _configuration.Channels)
        {
            try
            {
                var snapshotName = GetChannelSnapshotName(channel);

                var videosSnapshot = 
                    await _snapshotsRepository.Get<Video[]>(snapshotName, cancellationToken);

                if (videosSnapshot?.Data is not null)
                {
                    _log.LogInformation("videosSnapshot is not null");
                    return;
                }

                var videos = await _youTubeClient.SearchVideos(channel, cancellationToken);

                await UpdateSnapshot(snapshotName, videos, cancellationToken);
            }
            catch (Exception e)
            {
                _log.LogError(e, "Can't {methodName}", nameof(CreateVideosSnapshotIfNotExists));
            }
        }
    }

    private async Task UpdateSnapshot<TData>(string snapshotName, TData data, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        var newSnapshot = Snapshot<TData>.Create(snapshotName, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
        _log.LogInformation("Finish UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
    }

    private string GetChannelSnapshotName(string channel)
    {
        return $"{nameof(_youTubeClient.SearchVideos)}_{channel}";
    }
}