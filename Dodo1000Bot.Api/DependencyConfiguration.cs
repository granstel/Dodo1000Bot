﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dodo1000Bot.Api.DependencyModules;
using Dodo1000Bot.Services.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api
{
    internal static class DependencyConfiguration
    {
        internal static void Configure(IServiceCollection services, IConfiguration appConfiguration)
        {
            var configuration = appConfiguration.Get<AppConfiguration>();
            
            services.AddSingleton(configuration);
            services.AddSingleton(configuration.HttpLog);
            services.AddSingleton(configuration.PushNotifications);
            services.AddSingleton(configuration.UnitsJob);
            services.AddSingleton(configuration.StatisticsJob);
            services.AddSingleton(configuration.Management);
            services.AddSingleton(configuration.YouTube);

            services.AddInternalServices();
            services.AddJobs(configuration);

            services.AddExternalServices(configuration);
            services.AddMigrations(configuration);
            services.AddRepositories(configuration);

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
