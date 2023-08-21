using Dodo1000Bot.Api.Jobs;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules;

public static class JobsRegistration
{
    internal static void AddJobs(this IServiceCollection services, AppConfiguration appConfiguration)
    {
        services.AddHostedService(serviceProvider => new MigrationsJob(appConfiguration.MysqlConnectionString, serviceProvider));
        services.AddHostedService<FirstRunJob>();
        services.AddHostedService<PushNotificationsJob>();
        services.AddHostedService<UnitsCheckAndNotifyJob>();
        services.AddHostedService<StatisticsCheckAndNotifyJob>();
    }
}