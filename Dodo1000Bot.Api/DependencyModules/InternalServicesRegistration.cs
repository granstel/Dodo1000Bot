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
            services.AddTransient<UnitsService>();
            services.AddTransient<StatisticsService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IGlobalApiService, GlobalApiService>();

            services.AddScoped<IDialogflowService, DialogflowService>();
        }
    }
}
