using System.Reflection;
using Dodo1000Bot.Services.Configuration;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules;

public static class DatabaseRegistration
{
    internal static void AddMigrations(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb =>
            {
                rb
                    .AddMySql5()
                    .WithGlobalConnectionString(_ => configuration.MysqlConnectionString)
                    .WithMigrationsIn()
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations();
            });
    }
}