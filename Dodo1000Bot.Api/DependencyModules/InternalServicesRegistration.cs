using Dodo1000Bot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules
{
    internal static class InternalServicesRegistration
    {
        internal static void AddInternalServices(this IServiceCollection services)
        {
            services.AddTransient<IConversationService, ConversationService>();
            services.AddTransient<INotificationsService, NotificationsService>();
            services.AddScoped<UnitsService>();
            services.AddTransient<StatisticsService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddScoped<IGlobalApiService, GlobalApiService>();
            services.AddScoped<IPublicApiService, PublicApiService>();

            services.AddScoped<IDialogflowService, DialogflowService>();
        }
    }
}
