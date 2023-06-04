using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dodo1000Bot.Api.Extensions;
using Dodo1000Bot.Api.Middleware;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dodo1000Bot.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            InternalLoggerFactory.Factory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(logging =>
                {
                    logging.AddJsonConsole(o =>
                    {
                        var options = new JsonWriterOptions
                        {
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        };

                        if (_environment.IsDevelopment())
                        {
                            options.Indented = true;
                            o.TimestampFormat = "O";
                        }

                        o.JsonWriterOptions = options;
                    });
                })
                .AddSingleton(s =>
                {
                    var configureOptions = s.GetService<IConfigureOptions<JsonOptions>>();

                    var options = new JsonOptions();
                    configureOptions?.Configure(options);

                    return options.JsonSerializerOptions;
                })
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddHttpLogging(o =>
            {
                o.LoggingFields = HttpLoggingFields.All;
            });

            DependencyConfiguration.Configure(services, _configuration);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppConfiguration configuration)
        {
            app.UseMiddleware<ExceptionsMiddleware>();

            app.UseRouting();

            if (configuration.HttpLog.Enabled)
            {
                app.UseWhen(context => configuration.HttpLog.IncludeEndpoints.Any(context.ContainsEndpoint), a =>
                {
                    a.UseHttpLogging();
                });
            }

            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
