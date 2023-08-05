using System;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class StatisticsCheckAndNotifyJob: CheckAndNotifyJob<StatisticsService, StatisticsJobConfiguration>
{
    public StatisticsCheckAndNotifyJob(
        ILogger<StatisticsCheckAndNotifyJob> log, 
        IServiceProvider provider, 
        StatisticsJobConfiguration configuration) : base(log, provider, configuration)
    {
    }
}