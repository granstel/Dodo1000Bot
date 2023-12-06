using System;
using Dodo1000Bot.Api.Extensions;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Clients;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Dodo1000Bot.Services.Configuration;
using Grpc.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules
{
    internal static class ExternalServicesRegistration
    {
        internal static void AddExternalServices(this IServiceCollection services, AppConfiguration configuration)
        {
            services.AddSingleton<SessionsClient>(RegisterDialogflowSessionsClient);

            services.AddHttpClient<IGlobalApiClient, GlobalApiClient>(configuration.GlobalApiEndpoint, 
                nameof(GlobalApiClient));
            services.AddHttpClient<IPublicApiClient, PublicApiClient>(nameof(PublicApiClient));
            services.AddHttpClient<IRealtimeBoardApiClient, RealtimeBoardApiClient>(configuration.RealtimeBoardApiClientEndpoint, 
                nameof(configuration.RealtimeBoardApiClientEndpoint));
            services.AddHttpClient<IRestcountriesApiClient, RestcountriesApiClient>(configuration.RestcountriesApiClientEndpoint, 
                nameof(configuration.RestcountriesApiClientEndpoint));

            services.AddMemoryCache();
        }

        private static SessionsClient RegisterDialogflowSessionsClient(IServiceProvider provider)
        {
            var configuration = provider.GetService<DialogflowConfiguration>();

            var credential = GoogleCredential.FromFile(configuration.JsonPath).CreateScoped(SessionsClient.DefaultScopes);

            var clientBuilder = new SessionsClientBuilder
            {
                ChannelCredentials = credential.ToChannelCredentials()
            };

            var client = clientBuilder.Build();

            return client;
        }
    }
}
