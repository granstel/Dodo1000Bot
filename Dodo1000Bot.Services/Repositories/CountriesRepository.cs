using System.Threading;
using System.Threading.Tasks;
using Dapper;
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

        public async Task<string> GetName(string code, CancellationToken cancellationToken)
        {
            var name = await _connection.QueryFirstOrDefaultAsync<string>(
            @"SELECT Name FROM countries 
              WHERE Code = @code",
            new
            {
                code
            });

            return name;
        }

        public async Task Save(string code, string name, CancellationToken cancellationToken1)
        {
            await _connection.ExecuteAsync(
                "INSERT INTO countries (Code, Name) VALUES (@code, @name)" +
                "ON DUPLICATE KEY UPDATE Code = @code, Name = @name",
                new
                {
                    code,
                    name
                });
        }
    }
}
