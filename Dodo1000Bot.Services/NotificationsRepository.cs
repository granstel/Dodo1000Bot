using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services;

public class NotificationsRepository : INotificationsRepository
{
    private readonly MySqlConnection _connection;

    public NotificationsRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task Save(Notification notification, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(notification);
        await _connection.ExecuteAsync(
            "INSERT INTO notifications (payload) VALUES (@payload)",
            new
            {
                payload = payload
            });
    }
}