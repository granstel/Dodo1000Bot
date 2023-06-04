using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dodo1000Bot.Api.DependencyModules;
using Dodo1000Bot.Services.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Api
{
    internal static class DependencyConfiguration
    {
        internal static void Configure(IServiceCollection services, IConfiguration appConfiguration)
        {
            var configuration = appConfiguration.Get<AppConfiguration>();
            
            services.AddSingleton(configuration);
            services.AddSingleton(configuration.HttpLog);
            services.AddSingleton(configuration.Redis);
            services.AddSingleton(configuration.Dialogflow);
            services.AddSingleton(configuration.Units);

            services.AddSingleton(_ => new MySqlConnection(configuration.MysqlConnectionString));

            services.AddInternalServices();
            services.AddJobs();

            services.AddExternalServices(configuration);
            services.AddMigrations(configuration);

            var names = GetAssembliesNames();
            services.AddMapping(names);
        }


        public static ICollection<string> GetAssembliesNames()
        {
            var callingAssemble = Assembly.GetCallingAssembly();

            var names = callingAssemble.GetCustomAttributes<ApplicationPartAttribute>()
                .Where(a => a.AssemblyName.Contains("Dodo1000Bot", StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.AssemblyName).ToList();

            return names;
        }
    }
}
