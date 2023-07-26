using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Dodo1000Bot.Services.Tests.Repositories;

[Ignore("Integration")]
public class NotificationsRepositoryIntegrationTests
{
    private MySqlConnection _connection;

    private NotificationsRepository _target;

    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        var connectionString = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<NotificationsRepositoryIntegrationTests>()
            .Build()
            .GetSection("MysqlConnectionString").Get<string>();

        _connection = new MySqlConnection(connectionString);

        _target = new NotificationsRepository(_connection);

        _fixture = new Fixture { OmitAutoProperties = true };
    }

    [Test]
    public async Task Save_LongDistinction_TrimmedDistinction()
    {
        var chars = _fixture.CreateMany<char>(80).ToArray();
        var text = new string(chars);
        var payload = _fixture.Build<NotificationPayload>()
            .With(p => p.Text, text)
            .Create();
        var notification = _fixture.Build<Notification>()
            .With(n => n.Payload, payload)
            .Create();

        await _target.Save(notification, CancellationToken.None);
    }
}