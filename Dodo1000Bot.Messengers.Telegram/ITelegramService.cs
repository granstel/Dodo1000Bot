﻿using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Telegram.Bot.Types;

namespace Dodo1000Bot.Messengers.Telegram
{
    public interface ITelegramService : IMessengerService<Update, string>
    {
        Task<bool> TestApiAsync();

        Task<User> GetMeAsync();
    }
}