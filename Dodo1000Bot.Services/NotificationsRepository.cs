using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models;
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
            @"SELECT * FROM notifications n 
                 LEFT JOIN pushed_notifications pn 
                    ON n.id = pn.id
                  WHERE pn.id IS NULL");

        return notifications;
    }

    public async Task Save(IEnumerable<PushedNotification> pushedNotifications, CancellationToken cancellationToken)
    {
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