using Dapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services
{
    public class UsersRepository : IUsersRepository
    {
        private readonly MySqlConnection _connection;

        public UsersRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IList<User>> GetUsers(Source messengerType, CancellationToken cancellationToken)
        {
            var users = await _connection.QueryAsync<User>(
                @"SELECT Id, MessengerUserId FROM users
                  WHERE MessengerType = @messengerType",
                new
                {
                    messengerType
                });

            return users.ToImmutableArray();
        }

        public async Task SaveUser(User user, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(
                "INSERT INTO users (MessengerUserId, MessengerType) VALUES (@messengerUserId, @messengerType)",
                new
                {
                    messengerUserId = user.MessengerUserId,
                    messengerType = user.MessengerType
                });
        }

        public async Task<bool> IsExists(User user, CancellationToken cancellationToken)
        {
            var isExists = await _connection.QuerySingleAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM users WHERE MessengerUserId = @messengerUserId AND MessengerType = @messengerType) as isExists",
                new
                {
                    messengerUserId = user.MessengerUserId,
                    messengerType = user.MessengerType
                });

            return isExists;
        }
    }
}
