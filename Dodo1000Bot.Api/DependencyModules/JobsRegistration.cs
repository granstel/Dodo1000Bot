using System.Threading.Channels;
using Dodo1000Bot.Api.Jobs;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules;

public static class JobsRegistration
{
    internal static void AddJobs(this IServiceCollection services, AppConfiguration appConfiguration, bool isDevelopment)
    {
        services.AddHostedService(serviceProvider => new MigrationsJob(appConfiguration.MysqlConnectionString, serviceProvider));

        if (!isDevelopment)
        {
            services.AddHostedService<FirstRunJob>();
            services.AddHostedService<PushNotificationsRepeatableJob>();

            services.AddHostedService<UnitsCheckAndNotifyJob>();
            services.AddHostedService<StatisticsCheckAndNotifyJob>();
            services.AddHostedService<YoutubeCheckAndNotifyJob>();
        }

        var channelOptions = new BoundedChannelOptions(1_000)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
            SingleWriter = true
        };
        var channel = Channel.CreateBounded<Notification>(channelOptions);

        services.AddSingleton<ChannelWriter<Notification>>(channel);
        services.AddSingleton<ChannelReader<Notification>>(channel);

        services.AddHostedService<SaveNotificationsJob>();
    }
}