using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services;

public class CustomNotificationsRepository : ICustomNotificationsRepository
{
    private readonly MySqlConnection _connection;

    public CustomNotificationsRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task Save(Notification notification, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(notification?.Payload);

        await _connection.ExecuteAsync(new CommandDefinition(
            "INSERT IGNORE INTO custom_notifications (Payload) VALUES (@payload)",
            new
            {
                payload
            }, cancellationToken: cancellationToken));
    }

    public async Task<Notification> Get(int id, CancellationToken cancellationToken)
    {
        var record = await _connection.QueryFirstOrDefaultAsync(new CommandDefinition(
            "SELECT Id, Payload FROM custom_notifications WHERE Id = @id",
            new
            {
                id
            }, cancellationToken: cancellationToken));

        if (record is null)
        {
            return null;
        }

        var notification = new Notification(NotificationType.Custom)
        {
            Id = record.Id,
            Payload = JsonSerializer.Deserialize<NotificationPayload>(record.Payload)
        };

        return notification;
    }

    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        await _connection.ExecuteAsync(new CommandDefinition(
            "DELETE FROM custom_notifications WHERE Id = @id",
            new
            {
                id
            }, cancellationToken: cancellationToken));
    }
}