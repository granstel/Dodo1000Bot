using System.Collections.Generic;
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

    public async Task<IEnumerable<Notification>> GetNotPushedNotifications(CancellationToken cancellationToken)
    {
        var notifications = await _connection.QueryAsync<Notification>(
            @"SELECT * FROM notification n 
                 LEFT JOIN pushed_notifications pn 
                    ON n.id = pn.id
                  WHERE pn.id IS NULL");

        return notifications;
    }
}