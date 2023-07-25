using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.RealtimeBoard;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class StatisticsService : CheckAndNotifyService
{
    private readonly ILogger<StatisticsService> _log;
    private readonly IRealtimeBoardApiClient _realtimeBoardApiClient;
    private readonly INotificationsService _notificationsService;

    public StatisticsService(ILogger<StatisticsService> log, IRealtimeBoardApiClient realtimeBoardApiClient, 
        INotificationsService notificationsService)
    {
        _log = log;
        _realtimeBoardApiClient = realtimeBoardApiClient;
        _notificationsService = notificationsService;
    }

    public override async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        try
        {
            var statistics = await _realtimeBoardApiClient.Statistics(cancellationToken);

            await AboutOrdersPerMinute(statistics, cancellationToken);
            await AboutYearRevenue(statistics, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't check and notify statistics");
        }
    }

    private async Task AboutOrdersPerMinute(Statistics statistics, CancellationToken cancellationToken)
    {
        var ordersPerMinute = statistics.OrdersPerMinute;

        if (!CheckThe1000Rule(ordersPerMinute))
        {
            return;
        }

        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = $"There is {ordersPerMinute} orders per minute! See that on https://realtime.dodobrands.io",
                HappenedAt = DateTime.Now
            }
        };
        await _notificationsService.Save(notification, cancellationToken);
    }

    private async Task AboutYearRevenue(Statistics statistics, CancellationToken cancellationToken)
    {
        var yearRevenue = statistics.Revenues
            .Where(r => r.Type == RevenueTypes.Year)
            .Select(r => r.Revenue).FirstOrDefault();

        if (!CheckThe1000Rule(yearRevenue))
        {
            return;
        }

        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = $"There is {yearRevenue} dollars in revenue this year!"
            }
        };
        await _notificationsService.Save(notification, cancellationToken);
    }
}