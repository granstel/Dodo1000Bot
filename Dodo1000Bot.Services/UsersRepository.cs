﻿using Dapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
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

        public async Task<IEnumerable<User>> GetUsers(Source messengerType, CancellationToken cancellationToken)
        {
            var users = await _connection.QueryAsync<User>(
                @"SELECT Id, MessengerUserId FROM users
                  WHERE MessengerType = @messengerType",
                new
                {
                    messengerType
                });

            return users;
        }
    }
}
