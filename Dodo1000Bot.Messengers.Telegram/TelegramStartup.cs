using System;
using Dodo1000Bot.Messengers.Telegram;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

[assembly: HostingStartup(typeof(TelegramStartup))]
namespace Dodo1000Bot.Messengers.Telegram
{
    public class TelegramStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddConfiguration<TelegramConfiguration>("appsettings.Telegram.json", Source.Telegram.ToString());

                services.AddTransient<ITelegramService, TelegramService>();
                services.AddTransient<ITelegramBotClient>(RegisterTelegramClient);
                services.AddTransient<INotifyService, TelegramNotifyService>();
                
                services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new UnixDateTimeConverter());
                });
            });
        }

        private TelegramBotClient RegisterTelegramClient(IServiceProvider provider)
        {
            var configuration = provider.GetService<TelegramConfiguration>();

            var telegramClient = new TelegramBotClient(configuration.Token)
            {
                Timeout = TimeSpan.FromMinutes(3)
            };

            return telegramClient;
        }
    }
}
