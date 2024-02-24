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
            var users = await _connection.QueryAsync<User>(new CommandDefinition(
                @"SELECT Id, MessengerUserId, MessengerType, IsAdmin FROM users
                  WHERE MessengerType = @messengerType",
                new
                {
                    messengerType
                }, cancellationToken: cancellationToken));

            return users.ToImmutableArray();
        }

        public async Task SaveUser(User user, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(new CommandDefinition(
                "INSERT IGNORE INTO users (MessengerUserId, MessengerType) VALUES (@messengerUserId, @messengerType)",
                new
                {
                    messengerUserId = user.MessengerUserId,
                    messengerType = user.MessengerType
                }, cancellationToken: cancellationToken));
        }

        public async Task Delete(User user, CancellationToken cancellationToken)
        {
            await _connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM users WHERE MessengerUserId = @messengerUserId AND MessengerType = @messengerType",
                new
                {
                    messengerUserId = user.MessengerUserId,
                    messengerType = user.MessengerType
                }, cancellationToken: cancellationToken));
        }

        public async Task<int> Count(CancellationToken cancellationToken)
        {
            var count = await _connection.QuerySingleOrDefaultAsync<int>(new CommandDefinition(
                "SELECT COUNT(Id) FROM users", 
                cancellationToken: cancellationToken));

            return count;
        }

        public async Task<IList<User>> GetAdmins(Source messengerType, CancellationToken cancellationToken)
        {
            var users = await _connection.QueryAsync<User>(new CommandDefinition(
                @"SELECT Id, MessengerUserId, MessengerType FROM users
                  WHERE MessengerType = @messengerType AND IsAdmin = 1",
                new
                {
                    messengerType
                }, cancellationToken: cancellationToken));

            return users.ToImmutableArray();
        }
    }
}
