using System;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Dodo1000Bot.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddHttpClient<TClient, TImplementation>(this IServiceCollection services, string endpointUrl,
        string endpointName)where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>(c =>
        {
            if (!Uri.TryCreate(endpointUrl, UriKind.Absolute, out Uri result))
            {
                throw new ArgumentException($"Can't create Uri from {typeof(TImplementation).Name}. " +
                                            $"Value: {endpointUrl}");
            }

            c.BaseAddress = result;
        }).UseHttpClientMetrics();
    }
}