using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services
{
    public class ConversationService : IConversationService
    {
        private ILogger<ConversationService> _log;
        private readonly IUsersRepository _usersRepository;

        public ConversationService(ILogger<ConversationService> log, IUsersRepository usersRepository)
        {
            _log = log;
            _usersRepository = usersRepository;
        }

        public async Task<Response> GetResponseAsync(Request request, CancellationToken cancellationToken)
        {
            var response = new Response { Text = string.Empty };

            var user = new User
            {
                MessengerUserId = request.UserHash,
                MessengerType = request.Source
            };

            try
            {
                await _usersRepository.SaveUser(user, cancellationToken);
            }
            catch (Exception e)
            {
                _log.LogError(e, "Can't save user");
            }

            return response;
        }
    }
}
