using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services;

public class NotificationsRepository
{
    private readonly MySqlConnection _connection;

    public NotificationsRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task Save(string notification)
    {
        await _connection.ExecuteAsync(
            "INSERT INTO notification (payload) VALUES (@payload)",
            new
            {
                payload = notification
            });
    }
}