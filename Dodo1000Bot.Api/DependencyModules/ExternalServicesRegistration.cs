using Dodo1000Bot.Api.Extensions;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Clients;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules
{
    internal static class ExternalServicesRegistration
    {
        internal static void AddExternalServices(this IServiceCollection services, AppConfiguration configuration)
        {
            services.AddHttpClient<IGlobalApiClient, GlobalApiClient>(configuration.GlobalApiEndpoint, 
                nameof(configuration.GlobalApiEndpoint));
            services.AddHttpClient<IRealtimeBoardApiClient, RealtimeBoardApiClient>(configuration.RealtimeBoardApiClientEndpoint, 
                nameof(configuration.RealtimeBoardApiClientEndpoint));
            services.AddHttpClient<IRestcountriesApiClient, RestcountriesApiClient>(configuration.RestcountriesApiClientEndpoint, 
                nameof(configuration.RestcountriesApiClientEndpoint));
            services.AddHttpClient<IYouTubeClient, YouTubeClient>(configuration.YouTube.Endpoint, 
                nameof(configuration.RestcountriesApiClientEndpoint));
        }
    }
}
