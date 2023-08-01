using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Dodo1000Bot.Services.Tests.Repositories;

public class SnapshotsRepositoryTests
{
    private MySqlConnection _connection;

    private SnapshotsRepository _target;

    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        var connectionString = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<SnapshotsRepositoryTests>()
            .Build()
            .GetSection("MysqlConnectionString").Get<string>();

        _connection = new MySqlConnection(connectionString);

        _target = new SnapshotsRepository(_connection);

        _fixture = new Fixture { OmitAutoProperties = true };
    }

    [Test]
    [Ignore("Integration")]
    public async Task Save_DuplicateSnapshotName_UpdatedSnapshot()
    {
        var snapshot = _fixture.Build<Snapshot<string>>()
            .With(s => s.Id)
            .With(s => s.Name)
            .With(s => s.Data)
            .Create();

        await _target.Save(snapshot, CancellationToken.None);

        var data = _fixture.Create<string>();

        var newSnapshot = Snapshot<string>.Create(snapshot, data);

        await _target.Save(newSnapshot, CancellationToken.None);

        var dbSnapshot = await _target.Get<string>(snapshot.Name, CancellationToken.None);

        Assert.AreEqual(newSnapshot.Name, dbSnapshot.Name);
        Assert.AreEqual(newSnapshot.Data, dbSnapshot.Data);
    }
}