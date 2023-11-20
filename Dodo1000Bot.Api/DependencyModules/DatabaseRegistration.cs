using System.Reflection;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Api.DependencyModules;

public static class DatabaseRegistration
{
    internal static void AddMigrations(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(builder =>
            {
                builder
                    .AddMySql5()
                    .WithGlobalConnectionString(_ => configuration.MysqlConnectionString)
                    .WithMigrationsIn(Assembly.GetExecutingAssembly());
            });
    }

    internal static void AddRepositories(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddTransient(_ => new MySqlConnection(configuration.MysqlConnectionString));
        services.AddTransient<INotificationsRepository, NotificationsRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<ISnapshotsRepository, SnapshotsRepository>();
        services.AddTransient<ICountriesRepository, CountriesRepository>();
        services.AddTransient<ICustomNotificationsRepository, CustomNotificationsRepository>();
    }
}