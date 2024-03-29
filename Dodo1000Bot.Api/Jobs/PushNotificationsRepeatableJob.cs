﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class PushNotificationsRepeatableJob: RepeatableJob
{
    private readonly IServiceProvider _provider;

    public PushNotificationsRepeatableJob(
        ILogger<PushNotificationsRepeatableJob> log,
        PushNotificationsConfiguration configuration,
        IServiceProvider provider) : base(log, configuration.EveryTime)
    {
        _provider = provider;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();

        await notificationsService.PushNotifications(cancellationToken);
    }
}