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
    private readonly IServiceProvider _provider;

    public PushNotificationsJob(
        ILogger<PushNotificationsJob> log,
        INotificationsService notificationsService,
        PushNotificationsConfiguration configuration,
        IServiceProvider provider) : base(log, configuration.EveryTime)
    {
        _notificationsService = notificationsService;
        _provider = provider;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var notifyServices = scope.ServiceProvider.GetServices<INotifyService>();
        await _notificationsService.PushNotifications(notifyServices, cancellationToken);
    }
}