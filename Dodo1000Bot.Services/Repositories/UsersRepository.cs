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
                @"SELECT Id, MessengerUserId FROM users
                  WHERE MessengerType = @messengerType",
                new
                {
                    messengerType
                }, cancellationToken: cancellationToken));

            return users.ToImmutableArray();
        }

        public async Task<int> GetUserId(string messengerUserId, Source messengerType, CancellationToken cancellationToken)
        {
            var userId = await _connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(
                @"SELECT Id FROM users
                  WHERE MessengerUserId = @messengerUserId AND MessengerType = @messengerType",
                new
                {
                    messengerUserId,
                    messengerType
                }, cancellationToken: cancellationToken));

            return userId;
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
    }
}
