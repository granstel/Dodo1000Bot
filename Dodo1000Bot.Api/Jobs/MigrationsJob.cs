using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Api.Jobs;

public class MigrationsJob : IHostedService
{
    private readonly string _mySqlConnection;
    private readonly IServiceProvider _provider;

    public MigrationsJob(string mySqlConnection, IServiceProvider provider)
    {
        _mySqlConnection = mySqlConnection;
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await MigrateDatabase(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task MigrateDatabase(CancellationToken ct)
    {
        var rawConnectionString = _mySqlConnection;
        var (connectionString, databaseName) = StripDatabaseName(rawConnectionString);

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync(ct);
        await connection.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{databaseName}`");

        await using var scope = _provider.CreateAsyncScope();
        var runner = scope.ServiceProvider
            .GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    static (string connectionString, string databaseName) StripDatabaseName(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        builder.Database = null;
        return (builder.ConnectionString, databaseName);
    }
}