using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Dodo1000Bot.Messengers.Telegram
{
    public class TelegramController : MessengerController<Update, Response>
    {
        private readonly ITelegramService _telegramService;

        public TelegramController(
            ILogger<TelegramController> log, 
            ITelegramService telegramService, 
            TelegramConfiguration configuration, 
            [FromKeyedServices($"{nameof(TelegramController)}{nameof(JsonSerializerOptions)}")] JsonSerializerOptions jsonOptions)
            : base(log, telegramService, configuration)
        {
            _telegramService = telegramService;
            SerializerSettings = jsonOptions;
        }

        [HttpGet("TestTelegramApi")]
        public async Task<IActionResult> TestTelegramApiAsync()
        {
            var result = await _telegramService.TestApiAsync();

            return Json(result);
        }

        [HttpGet("GetMe")]
        public async Task<IActionResult> GetMeAsync()
        {
            var user = await _telegramService.GetMeAsync();

            return new JsonResult(user);
        }
        
        public new async Task<IActionResult> WebHook(string token, CancellationToken cancellationToken)
        {
            
                // using var memoryReader = new StreamReader(HttpContext.Request.Body);
                // var bodyReadResult = await memoryReader.ReadToEndAsync(cancellationToken);
                var update= await JsonSerializer.DeserializeAsync<Update>(HttpContext.Request.Body, SerializerSettings as JsonSerializerOptions, cancellationToken);
            var response = await _telegramService.ProcessIncomingAsync(update, cancellationToken);

            return Json(response, SerializerSettings);
        }
    }
}