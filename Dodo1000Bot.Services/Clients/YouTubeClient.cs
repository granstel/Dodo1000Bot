using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Youtube;
using Dodo1000Bot.Services.Configuration;

namespace Dodo1000Bot.Services.Clients;

public class YouTubeClient : IYouTubeClient
{
    private readonly YoutubeConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions  _serializerOptions;

    public YouTubeClient(YoutubeConfiguration configuration, HttpClient httpClient, JsonSerializerOptions serializerOptions)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _serializerOptions = serializerOptions;
    }

    public async Task<Video[]> SearchVideos(string channelId, CancellationToken cancellationToken)
    {
        var url = $"?part=snippet&channelId={channelId}&order=date&type=video&maxResults=5&key={_configuration.ApiKey}";

        var searchResponse = await _httpClient.GetFromJsonAsync<SearchResponse>(url, _serializerOptions, cancellationToken);

        return searchResponse.Items;
    }
}