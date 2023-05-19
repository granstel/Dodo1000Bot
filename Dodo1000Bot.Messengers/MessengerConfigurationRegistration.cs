using System;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Messengers
{
    public static class MessengerConfigurationRegistration
    {
        public static void AddConfiguration<T>(this IServiceCollection services, string fileName) where T : MessengerConfiguration
        {
            services.AddSingleton(context =>
            {
                const string extension = ".json";

                if (fileName.IndexOf(extension, StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    fileName = $"{fileName}{extension}";
                }

                var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(fileName, true, false);

                var configurationRoot = configurationBuilder.Build();

                var configuration = configurationRoot.Get<T>();

                return configuration;
            });
        }
    }
}
