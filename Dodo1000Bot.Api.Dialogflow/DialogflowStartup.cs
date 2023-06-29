using Dodo1000Bot.Api.Dialogflow;
using Dodo1000Bot.Messengers;
using Dodo1000Bot.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DialogflowStartup))]
namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddConfiguration<DialogflowConfiguration>("appsettings.Dialogflow.json", Source.Telegram.ToString());

            services.AddTransient<IDialogflowService, DialogflowService>();
        });
    }
}