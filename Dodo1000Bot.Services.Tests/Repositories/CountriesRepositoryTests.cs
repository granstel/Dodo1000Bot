using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Dodo1000Bot.Models.GlobalApi;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Dodo1000Bot.Services.Tests.Repositories;

public class CountriesRepositoryTests
{
    private MySqlConnection _connection;

    private CountriesRepository _target;

    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        var connectionString = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<CountriesRepositoryTests>()
            .Build()
            .GetSection("MysqlConnectionString").Get<string>();

        _connection = new MySqlConnection(connectionString);

        _target = new CountriesRepository(_connection);

        _fixture = new Fixture { OmitAutoProperties = true };
    }

    [Test]
    [Ignore("Integration")]
    public async Task Save_DuplicateCountryId_UpdatedCountry()
    {
        var country = _fixture.Build<UnitCountModel>()
            .With(s => s.CountryId)
            .With(s => s.CountryName)
            .Create();

        await _target.Save(country, CancellationToken.None);

        var newName = _fixture.Create<string>();

        country.CountryName = newName;

        await _target.Save(country, CancellationToken.None);

        var name = await _target.GetName(country.CountryId, CancellationToken.None);

        Assert.AreEqual(newName, name);
    }
}