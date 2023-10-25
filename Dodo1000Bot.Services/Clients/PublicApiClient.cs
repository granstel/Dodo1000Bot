using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.PublicApi;
using Dodo1000Bot.Services.Configuration;
using Dodo1000Bot.Services.Extensions;

namespace Dodo1000Bot.Services.Clients;

public class PublicApiClient : IPublicApiClient
{
    private readonly HttpClient _httpClient;
    private readonly PublicApiEndpoints _endpoints;
    private readonly JsonSerializerOptions  _serializerOptions;

    public PublicApiClient(HttpClient httpClient, PublicApiEndpoints endpoints, JsonSerializerOptions serializerOptions)
    {
        _httpClient = httpClient;
        _endpoints = endpoints;
        _serializerOptions = serializerOptions;
    }

    public async Task<UnitInfo[]> UnitInfo(Brands brand, string countryCode, CancellationToken cancellationToken)
    {
        var endpoint = _endpoints.GetValueOrDefault(brand);
        var url = $"{endpoint}{countryCode}/api/v1/unitinfo";

        var unitsInfo = await _httpClient.GetAsync<UnitInfo[]>(url, _serializerOptions, cancellationToken);

        return unitsInfo;
    }
}