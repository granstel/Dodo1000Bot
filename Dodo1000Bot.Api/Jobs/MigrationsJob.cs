using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Api.Jobs;

public class MigrationsJob : BackgroundService
{
    private readonly MySqlConnection _mySqlConnection;
    private readonly IServiceProvider _provider;

    public MigrationsJob(MySqlConnection mySqlConnection, IServiceProvider provider)
    {
        _mySqlConnection = mySqlConnection;
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await MigrateDatabase(ct);
    }

    private async Task MigrateDatabase(CancellationToken ct)
    {
        var rawConnectionString = _mySqlConnection.ConnectionString;
        var databaseName = StripDatabaseName(rawConnectionString);

        await _mySqlConnection.OpenAsync(ct);
        await _mySqlConnection.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{databaseName}`");

        await using var scope = _provider.CreateAsyncScope();
        var runner = scope.ServiceProvider
            .GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    static string StripDatabaseName(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        builder.Database = null;
        return databaseName;
    }
}