using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.RealtimeBoard;

namespace Dodo1000Bot.Services;

public class StatisticsService : CheckAndNotifyService
{
    private readonly IRealtimeBoardApiClient _realtimeBoardApiClient;
    private readonly INotificationsService _notificationsService;

    public StatisticsService(IRealtimeBoardApiClient realtimeBoardApiClient, INotificationsService notificationsService)
    {
        _realtimeBoardApiClient = realtimeBoardApiClient;
        _notificationsService = notificationsService;
    }

    public override async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        var statistics = await _realtimeBoardApiClient.Statistics(cancellationToken);

        await AboutOrdersPerMinute(statistics, cancellationToken);
        await AboutYearRevenue(statistics, cancellationToken);
    }

    private async Task AboutOrdersPerMinute(Statistics unitsCount, CancellationToken cancellationToken)
    {
        var ordersPerMinute = unitsCount.OrdersPerMinute;

        if (!CheckTheRule(ordersPerMinute))
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

    private async Task AboutYearRevenue(Statistics unitsCount, CancellationToken cancellationToken)
    {
        var yearRevenue = unitsCount.Revenues.Where(r => r.Type == RevenueTypes.Year).Select(r => r.Revenue).FirstOrDefault();

        if (!CheckTheRule(yearRevenue))
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