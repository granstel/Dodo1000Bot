using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class PushNotificationsJob: RepeatableJob
{
    private readonly IServiceProvider _provider;

    public PushNotificationsJob(
        ILogger<PushNotificationsJob> log,
        PushNotificationsConfiguration configuration,
        IServiceProvider provider) : base(log, configuration.EveryTime)
    {
        _provider = provider;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var notifyServices = scope.ServiceProvider.GetServices<INotifyService>();
        var notificationsService = scope.ServiceProvider.GetService<INotificationsService>();

        await notificationsService.PushNotifications(notifyServices, cancellationToken);
    }
}