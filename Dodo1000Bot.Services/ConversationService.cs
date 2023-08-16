using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services
{
    public class ConversationService : IConversationService
    {
        private ILogger<ConversationService> _log;

        public ConversationService(ILogger<ConversationService> log, IUsersRepository usersRepository)
        {
            _log = log;
        }

        public Task<Response> GetResponseAsync(Request request, CancellationToken cancellationToken)
        {
            var response = new Response { Text = string.Empty };

            return Task.FromResult(response);
        }
    }
}
