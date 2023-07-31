using Dapper;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services.Interfaces;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services.Repositories
{
    public class SnapshotsRepository : ISnapshotsRepository
    {
        private readonly MySqlConnection _connection;

        public SnapshotsRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task Save<TData>(Snapshot<TData> snapshot, CancellationToken cancellationToken)
        {
            var data = JsonSerializer.Serialize(snapshot.Data);

            await _connection.ExecuteAsync(
                "INSERT INTO snapshots (name, data, modifiedAt) VALUES (@name, @data, @modifiedAt)" +
                "ON DUPLICATE KEY UPDATE name = @name, data = @data, modifiedAt = @modifiedAt",
                new
                {
                    name = snapshot.Name,
                    data,
                    modifiedAt = snapshot.ModifiedAt
                });
        }
    }
}
