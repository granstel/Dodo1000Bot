using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services;

public class NotificationTemplatesRepository : INotificationTemplatesRepository
{
    private readonly MySqlConnection _connection;

    public NotificationTemplatesRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<NotificationTemplate> GetRandom(
        NotificationType notificationType,
        Source messengerType,
        string languageCode,
        CancellationToken cancellationToken)
    {
        var template = await _connection.QueryFirstOrDefaultAsync<NotificationTemplate?>(new CommandDefinition(
            "SELECT * FROM notification_templates WHERE" +
            "NotificationType = @notificationType AND" +
            "MessengerType = @messengerType AND" +
            "LanguageCode = @languageCode" +
            "ORDER BY RAND() LIMIT 1",
            new
            {
                notificationType,
                messengerType,
                languageCode
            }, cancellationToken: cancellationToken));

        return template;
    }
}