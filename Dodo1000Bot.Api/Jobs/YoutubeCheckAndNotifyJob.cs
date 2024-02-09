using System;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class YoutubeCheckAndNotifyJob: CheckAndNotifyJob<YoutubeService, YoutubeJobConfiguration>
{
    public YoutubeCheckAndNotifyJob(
        ILogger<YoutubeCheckAndNotifyJob> log, 
        IServiceProvider provider, 
        YoutubeJobConfiguration configuration) : base(log, provider, configuration)
    {
    }
}