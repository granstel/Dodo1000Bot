using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models.Domain;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services;

public class NotificationsRepository : INotificationsRepository
{
    private readonly MySqlConnection _connection;

    public NotificationsRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task Save(NotificationPayload notificationPayload, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(notificationPayload);
        await _connection.ExecuteAsync(
            "INSERT INTO notifications (Payload) VALUES (@payload)",
            new
            {
                payload
            });
    }

    public async Task<IEnumerable<Notification>> GetNotPushedNotifications(CancellationToken cancellationToken)
    {
        var records = await _connection.QueryAsync(
            @"SELECT n.Id, n.Payload FROM notifications n 
                 LEFT JOIN pushed_notifications pn 
                    ON n.Id = pn.Id
                  WHERE pn.id IS NULL");

        var notifications = records.Select(r => new Notification
        {
            Id = r.Id,
            Payload = JsonSerializer.Deserialize<NotificationPayload>(r.Payload)
        });
        
        return notifications;
    }

    public async Task Save(IEnumerable<PushedNotification> pushedNotifications, CancellationToken cancellationToken)
    {
        await _connection.OpenAsync(cancellationToken);
        var transaction = await _connection.BeginTransactionAsync(cancellationToken);

        foreach(var pushedNotification in pushedNotifications)
        {
            await _connection.ExecuteAsync(
            "INSERT INTO pushed_notifications (NotificationId, UserId) VALUES (@notificationId, @userId)",
            new
            {
                notificationId = pushedNotification.NotificationId,
                userId = pushedNotification.UserId
            }, transaction);
        }

        await transaction.CommitAsync(cancellationToken);
    }
}