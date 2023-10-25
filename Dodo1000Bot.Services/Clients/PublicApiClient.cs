using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.PublicApi;
using Dodo1000Bot.Services.Extensions;

namespace Dodo1000Bot.Services.Clients;

public class PublicApiClient : IPublicApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions  _serializerOptions;

    public PublicApiClient(HttpClient httpClient, JsonSerializerOptions serializerOptions)
    {
        _httpClient = httpClient;
        _serializerOptions = serializerOptions;
    }

    public async Task<UnitInfo[]> UnitInfo(string countryCode, CancellationToken cancellationToken)
    {
        var url = $"{countryCode}/api/v1/unitinfo";

        var unitsInfo = await _httpClient.GetAsync<UnitInfo[]>(url, _serializerOptions, cancellationToken);

        return unitsInfo;
    }
}