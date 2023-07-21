using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class UnitsJob : RepeatableJob
{
    private readonly IServiceProvider _provider;

    public UnitsJob(ILogger<UnitsJob> log,
        IServiceProvider provider,
        UnitsConfiguration unitsConfiguration) : base(log, unitsConfiguration.RefreshEveryTime)
    {
        _provider = provider;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var unitsService = scope.ServiceProvider.GetRequiredService<UnitsService>();

        await unitsService.CheckAndNotify(cancellationToken);
    }
}