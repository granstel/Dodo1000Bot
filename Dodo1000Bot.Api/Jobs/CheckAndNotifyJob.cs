using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class CheckAndNotifyJob<TService, TConfiguration> : RepeatableJob 
    where TService : ICheckAndNotifyService 
    where TConfiguration : CheckAndNotifyJobConfiguration
{
    private readonly IServiceProvider _provider;

    public CheckAndNotifyJob(ILogger<CheckAndNotifyJob<TService, TConfiguration>> log,
        IServiceProvider provider,
        TConfiguration configuration) : base(log, configuration.RefreshEveryTime)
    {
        _provider = provider;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var checkAndNotifyService = scope.ServiceProvider.GetRequiredService<TService>();

        await checkAndNotifyService.CheckAndNotify(cancellationToken);
    }
}