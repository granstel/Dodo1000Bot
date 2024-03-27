using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Hosting;

namespace Dodo1000Bot.Api.Jobs;

public class SaveNotificationsJob : BackgroundService
{
    private readonly ChannelReader<Notification> _notificationsChannel;
    private readonly NotificationsService _notificationsService;

    public SaveNotificationsJob(ChannelReader<Notification> notificationsChannel, NotificationsService notificationsService)
    {
        _notificationsChannel = notificationsChannel;
        _notificationsService = notificationsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var notification in _notificationsChannel.ReadAllAsync(stoppingToken))
        {
            await _notificationsService.Save(notification, stoppingToken);
        }
    }
}