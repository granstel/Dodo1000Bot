using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class PushNotificationsJob: RepeatableJob
{
    private readonly INotificationsService _notificationsService;
    public PushNotificationsJob(ILogger<PushNotificationsJob> log, TimeSpan repeatEveryTime, INotificationsService notificationsService) : base(log, repeatEveryTime)
    {
        _notificationsService = notificationsService;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await _notificationsService.PushNotifications(cancellationToken);
    }
}