using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Dodo1000Bot.Messengers.Telegram
{
    public class TelegramService : MessengerService<Update, Response>, ITelegramService
    {
        private readonly ITelegramBotClient _client;

        public TelegramService(
            ILogger<TelegramService> log,
            ITelegramBotClient client,
            IConversationService conversationService,
            IMapper mapper) : base(log, conversationService, mapper)
        {
            _client = client;
        }

        public async Task<User> GetMeAsync()
        {
            var result = await _client.GetMeAsync();

            return result;
        }

        public async Task<bool> TestApiAsync()
        {
            var result = await _client.TestApiAsync();

            return result;
        }

        public override async Task<bool> SetWebhookAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                await _client.SetWebhookAsync(url);
            }
            catch (Exception e)
            {
                Log.LogError(e, "Error while set webhook");

                return false;
            }

            return true;
        }

        public override async Task<bool> DeleteWebhookAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _client.DeleteWebhookAsync();

                return true;
            }
            catch (Exception e)
            {
                Log.LogError(e, "Error while delete webhook");
            }

            return false;
        }

        protected override async Task<Response> AfterAsync(Update input, Response response)
        {
            var answer = response.Text;

            await SendTextMessageAsync(long.Parse(response.ChatHash), answer);

            return response;
        }

        private async Task SendTextMessageAsync(long chatId, string text)
        {
            try
            {
                await _client.SendTextMessageAsync(chatId, text);
            }
            catch (Exception e)
            {
                Log.LogError(e, "Error while send text message");
            }
        }
    }
}
