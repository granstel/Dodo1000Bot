using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.RealtimeBoard;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Dodo1000Bot.Services.Tests")]

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

    internal async Task AboutOrdersPerMinute(Statistics statistics, CancellationToken cancellationToken)
    {
        var ordersPerMinute = statistics.OrdersPerMinute;

        if (!CheckGreaterOrEqual1000(ordersPerMinute))
        {
            return;
        }

        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = "😮",
                HappenedAt = DateTime.Now
            }
        };

        await _notificationsService.Save(notification, cancellationToken);

        notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = "📈 Wow! There are more than 1000 orders per minute! See that on https://realtime.dodobrands.io",
                HappenedAt = DateTime.Now
            }
        };

        await _notificationsService.Save(notification, cancellationToken);
    }

    internal async Task AboutYearRevenue(Statistics statistics, CancellationToken cancellationToken)
    {
        var yearRevenue = statistics.Revenues
            .Where(r => r.Type == RevenueTypes.Year)
            .Select(r => r.Revenue).FirstOrDefault();

        const int givenRevenue = 1_000_000_000;

        if (!CheckGreaterOrEqualGivenValue(yearRevenue, givenRevenue))
        {
            return;
        }

        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = $"💰 There is over 1 000 000 000 dollars revenue in {DateTime.Now.Year} year! " +
                       $"See that on https://realtime.dodobrands.io"
            }
        };

        await _notificationsService.Save(notification, cancellationToken);
    }
}