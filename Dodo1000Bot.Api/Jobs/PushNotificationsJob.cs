using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class PushNotificationsJob: RepeatableJob
{
    private readonly INotificationsService _notificationsService;
    private readonly IEnumerable<INotifyService> _notifyServices;
    private readonly IServiceProvider _provider;

    public PushNotificationsJob(
        ILogger<PushNotificationsJob> log,
        INotificationsService notificationsService,
        IEnumerable<INotifyService> notifyServices,
        PushNotificationsConfiguration configuration,
        IServiceProvider provider) : base(log, configuration.EveryTime)
    {
        _notificationsService = notificationsService;
        _notifyServices = notifyServices;
        _provider = provider;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var notifyServices = scope.ServiceProvider.GetServices<INotifyService>();
        await _notificationsService.PushNotifications(_notifyServices, cancellationToken);
    }
}