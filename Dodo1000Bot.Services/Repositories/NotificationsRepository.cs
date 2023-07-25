using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
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
    private const int DistinctionMaxLength = 64;
    private readonly MySqlConnection _connection;

    public NotificationsRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task Save(Notification notification, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(notification?.Payload);

        var distinction = notification?.Distinction;
        if (distinction is { Length: > DistinctionMaxLength })
        {
            distinction = distinction[..DistinctionMaxLength];
        }

        await _connection.ExecuteAsync(
            "INSERT INTO notifications (Payload, Distinction) VALUES (@payload, @distinction)",
            new
            {
                payload,
                distinction
            });
    }

    public async Task<bool> IsExists(Notification notification, CancellationToken cancellationToken)
    {
        var isExists = await _connection.QuerySingleAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM notifications WHERE Distinction = @distinction) as isExists",
            new
            {
                distinction = notification?.Distinction
            });

        return isExists;
    }

    public async Task<IList<Notification>> GetNotPushedNotifications(CancellationToken cancellationToken)
    {
        var records = await _connection.QueryAsync(
            @"SELECT n.Id, n.Payload FROM notifications n 
                 LEFT JOIN pushed_notifications pn 
                    ON n.Id = pn.notificationId
                  WHERE pn.id IS NULL");

        var notifications = records.Select(r => new Notification
        {
            Id = r.Id,
            Payload = JsonSerializer.Deserialize<NotificationPayload>(r.Payload)
        }).ToImmutableArray();

        return notifications;
    }

    public async Task Save(IEnumerable<PushedNotification> pushedNotifications, CancellationToken cancellationToken)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }

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
        await _connection.CloseAsync();
    }
}