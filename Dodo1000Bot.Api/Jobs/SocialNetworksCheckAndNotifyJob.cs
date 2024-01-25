using System;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class SocialNetworksCheckAndNotifyJob: CheckAndNotifyJob<UnitsService, SocialNetworksJobConfiguration>
{
    public SocialNetworksCheckAndNotifyJob(
        ILogger<SocialNetworksCheckAndNotifyJob> log, 
        IServiceProvider provider, 
        SocialNetworksJobConfiguration configuration) : base(log, provider, configuration)
    {
    }
}