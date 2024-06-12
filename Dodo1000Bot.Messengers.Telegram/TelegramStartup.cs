using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dodo1000Bot.Messengers.Telegram;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
                services.AddSingleton<ITelegramBotClient>(RegisterTelegramClient);
                services.AddTransient<INotifyService, TelegramNotifyService>();

                var jsonOptions = new JsonOptions
                {
                    JsonSerializerOptions =
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    }
                };
                jsonOptions.JsonSerializerOptions.Converters.Add(new UnixDateTimeConverter());
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                services.Configure<JsonOptions>(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new UpdateConverter(jsonOptions.JsonSerializerOptions));
                });

                services.AddKeyedSingleton($"{nameof(TelegramController)}{nameof(JsonSerializerOptions)}", jsonOptions.JsonSerializerOptions);
            });
        }

        private TelegramBotClient RegisterTelegramClient(IServiceProvider provider)
        {
            var options = provider.GetKeyedService<JsonSerializerOptions>(
                $"{nameof(TelegramController)}{nameof(JsonSerializerOptions)}");
            var configuration = provider.GetService<TelegramConfiguration>();

            var telegramClient = new TelegramBotClient(configuration.Token)
            {
                Timeout = TimeSpan.FromMinutes(3)
            };

            return telegramClient;
        }
    }
}
