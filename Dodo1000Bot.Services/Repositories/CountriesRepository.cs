using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dodo1000Bot.Models.GlobalApi;
using MySql.Data.MySqlClient;

namespace Dodo1000Bot.Services
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly MySqlConnection _connection;

        public CountriesRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> GetName(int id, CancellationToken cancellationToken)
        {
            var name = await _connection.QueryFirstOrDefaultAsync<string>(
            @"SELECT Name FROM countries 
              WHERE id = @id",
            new
            {
                id
            });

            return name;
        }

        public async Task Save(UnitCountModel country, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(
                "INSERT INTO countries (Id, Name) VALUES (@id, @name)" +
                "ON DUPLICATE KEY UPDATE Id = @id, Name = @name",
                new
                {
                    id = country.CountryId,
                    name = country.CountryName,
                });
        }
    }
}
