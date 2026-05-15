using System;
using System.Net.Http;
using System.Threading;
using Dodo1000Bot.Messengers.Telegram;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Telegram.Bot;

[assembly: HostingStartup(typeof(TelegramStartup))]
namespace Dodo1000Bot.Messengers.Telegram
{
    public class TelegramStartup : IHostingStartup
    {
        private const string TelegramHttpClientName = "telegram-bot";

        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddConfiguration<TelegramConfiguration>("appsettings.Telegram.json", Source.Telegram.ToString());

                services.AddHttpClient(TelegramHttpClientName, client =>
                    {
                        client.Timeout = TimeSpan.FromMinutes(3);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                    {
                        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
                    })
                    .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
                    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));

                services.AddTransient<ITelegramService, TelegramService>();
                services.AddSingleton<ITelegramBotClient>(RegisterTelegramClient);
                services.AddTransient<INotifyService, TelegramNotifyService>();
            });
        }

        private static TelegramBotClient RegisterTelegramClient(IServiceProvider provider)
        {
            var configuration = provider.GetRequiredService<TelegramConfiguration>();
            var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(TelegramHttpClientName);

            return new TelegramBotClient(configuration.Token, httpClient);
        }
    }
}
