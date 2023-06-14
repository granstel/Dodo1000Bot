using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class PushNotificationsJob: RepeatableJob
{
    private readonly INotificationsService _notificationsService;
    private readonly IEnumerable<INotifyService> _notifyServices;

    public PushNotificationsJob(
        ILogger<PushNotificationsJob> log, 
        INotificationsService notificationsService,
        IEnumerable<INotifyService> notifyServices,
        PushNotificationsConfiguration configuration) : base(log, configuration.EveryTime)
    {
        _notificationsService = notificationsService;
        _notifyServices = notifyServices;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await _notificationsService.PushNotifications(_notifyServices, cancellationToken);
    }
}