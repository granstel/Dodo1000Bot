using Dodo1000Bot.Api.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules;

public static class JobsRegistration
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddHostedService<UnitsJob>();
    }
}