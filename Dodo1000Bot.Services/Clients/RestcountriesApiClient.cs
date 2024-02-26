using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Restcountries;

namespace Dodo1000Bot.Services.Clients;

public class RestcountriesApiClient : IRestcountriesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions  _serializerOptions;

    public RestcountriesApiClient(HttpClient httpClient, JsonSerializerOptions serializerOptions)
    {
        _httpClient = httpClient;
        _serializerOptions = serializerOptions;
    }

    public async Task<Country[]> GetNames(CancellationToken cancellationToken, params string[] codes)
    {
        var joinedCodes = string.Join(",", codes);
        var url = $"alpha?codes={joinedCodes}&fields=name";

        var countries = await _httpClient.GetFromJsonAsync<Country[]>(url, _serializerOptions, cancellationToken);

        return countries;
    }
}