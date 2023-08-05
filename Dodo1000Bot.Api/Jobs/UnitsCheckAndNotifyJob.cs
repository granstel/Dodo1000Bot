using System;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class UnitsCheckAndNotifyJob: CheckAndNotifyJob<UnitsService, UnitsJobConfiguration>
{
    public UnitsCheckAndNotifyJob(
        ILogger<UnitsCheckAndNotifyJob> log, 
        IServiceProvider provider, 
        UnitsJobConfiguration configuration) : base(log, provider, configuration)
    {
    }
}