using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public abstract class RepeatableJob : BackgroundService
{
    private readonly TimeSpan _repeatEveryTime;
    private readonly string _jobName;

    private ILogger Log { get; }

    protected RepeatableJob(ILogger log, TimeSpan repeatEveryTime)
    {
        Log = log;
        _repeatEveryTime = repeatEveryTime;
        _jobName = GetType().Name;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.LogInformation("Start {jobName}", _jobName);

        await TryExecuteJob(cancellationToken);

        await base.StartAsync(cancellationToken);

        Log.LogInformation("{jobName} started", _jobName);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(_repeatEveryTime, cancellationToken);

            await TryExecuteJob(cancellationToken);
        }

        Log.LogInformation("{jobName} finished", _jobName);
    }

    private async Task TryExecuteJob(CancellationToken cancellationToken)
    {
        try
        {
            Log.LogInformation("Execute {jobName}", _jobName);

            await Execute(cancellationToken);

            Log.LogInformation("{jobName} executed", _jobName);
        }
        catch (Exception e)
        {
            Log.LogError(e, "Error while executing {jobName}",_jobName);
        }
    }

    protected abstract Task Execute(CancellationToken cancellationToken);
}