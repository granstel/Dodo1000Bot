﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services.Extensions;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services;

public class NotificationsRepository : INotificationsRepository
{
    private readonly ILogger<NotificationsRepository> _logger;
    private readonly MySqlConnection _connection;

    public NotificationsRepository(ILogger<NotificationsRepository> logger, MySqlConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task Save(Notification notification, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(notification?.Payload);
        _logger.LogInformation("Save to DB: {payload}, {distinction}", payload, notification?.Distinction);
        var rowsAffected = await _connection.ExecuteAsync(new CommandDefinition(
            "INSERT IGNORE INTO notifications (Type, Payload, Distinction) VALUES (@type, @payload, @distinction)",
            new
            {
                notification?.Type,
                payload,
                notification?.Distinction
            }, cancellationToken: cancellationToken));
        _logger.LogInformation("{rowsAffected} rows affected", rowsAffected);
    }

    public async Task<IList<Notification>> GetNotPushedNotifications(CancellationToken cancellationToken)
    {
        var records = await _connection.QueryAsync(new CommandDefinition(
            @"SELECT n.Id, n.Type, n.Payload FROM notifications n 
                 LEFT JOIN pushed_notifications pn 
                    ON n.Id = pn.notificationId
                  WHERE pn.id IS NULL", cancellationToken: cancellationToken));

        
        var notifications = records.Select(r =>
        {
            var type = Enum.Parse<NotificationType>(r.Type.ToString());
            return new Notification(type)
            {
                Id = r.Id,
                Payload = JsonSerializer.Deserialize<NotificationPayload>(r.Payload)
            };
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
            await _connection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO pushed_notifications (NotificationId, UserId) VALUES (@notificationId, @userId)",
            new
            {
                notificationId = pushedNotification.NotificationId,
                userId = pushedNotification.UserId
            }, transaction, cancellationToken: cancellationToken));
        }

        await transaction.CommitAsync(cancellationToken);
        await _connection.CloseAsync();
    }

    public async Task Delete(int notificationId, CancellationToken cancellationToken)
    {
        await _connection.ExecuteAsync(new CommandDefinition(
            "DELETE FROM notifications WHERE Id = @notificationId",
            new
            {
                notificationId
            }, cancellationToken: cancellationToken));
    }
}