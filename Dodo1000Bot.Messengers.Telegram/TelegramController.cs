using System.Text.Json;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Microsoft.AspNetCore.Mvc;
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
            JsonSerializerOptions jsonOptions)
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
    }
}