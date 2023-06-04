using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Api.Jobs;

public class MigrationsJob : BackgroundService
{
    private readonly MySqlConnection _mySqlConnection;
    private readonly IMigrationRunner _migrationRunner;

    public MigrationsJob(MySqlConnection mySqlConnection, IMigrationRunner migrationRunner)
    {
        _mySqlConnection = mySqlConnection;
        _migrationRunner = migrationRunner;
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
        _migrationRunner.MigrateUp();
    }

    static string StripDatabaseName(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        builder.Database = null;
        return databaseName;
    }
}