using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class StatisticsJob : RepeatableJob
{
    private readonly IServiceProvider _provider;

    public StatisticsJob(ILogger<StatisticsJob> log,
        IServiceProvider provider,
        StatisticsJobConfiguration configuration) : base(log, configuration.RefreshEveryTime)
    {
        _provider = provider;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await using var scope = _provider.CreateAsyncScope();
    }
}