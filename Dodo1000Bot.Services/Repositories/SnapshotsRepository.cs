using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services
{
    public class SnapshotsRepository : ISnapshotsRepository
    {
        private readonly MySqlConnection _connection;

        public SnapshotsRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<Snapshot<TData>> Get<TData>(string snapshotName, CancellationToken cancellationToken)
        {
            var record = await _connection.QueryFirstOrDefaultAsync(
            @"SELECT * FROM snapshots 
              WHERE name = @name",
            new
            {
                name = snapshotName,
            });

            var snapshot = new Snapshot<TData>
            {
                Id = record.Id,
                Name = record.Name,
                Data = JsonSerializer.Deserialize<TData>(record.Data)
            };

            return snapshot;
        }

        public async Task Save<TData>(Snapshot<TData> snapshot, CancellationToken cancellationToken)
        {
            var data = JsonSerializer.Serialize(snapshot.Data);

            await _connection.ExecuteAsync(
                "INSERT INTO snapshots (name, data, modifiedAt) VALUES (@name, @data, @modifiedAt)" +
                "ON DUPLICATE KEY UPDATE data = @data, modifiedAt = @modifiedAt",
                new
                {
                    name = snapshot.Name,
                    data,
                    modifiedAt = snapshot.ModifiedAt
                });
        }
    }
}
