﻿using System;
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
        var (connectionString, databaseName) = StripDatabaseName(_mySqlConnection);

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await connection.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{databaseName}`");

        await using var scope = _provider.CreateAsyncScope();
        var runner = scope.ServiceProvider
            .GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    static (string connectionString, string databaseName) StripDatabaseName(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        builder.Database = null;
        return (builder.ConnectionString, databaseName);
    }
}