using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.RealtimeBoard;
using Dodo1000Bot.Services.Configuration;
using Dodo1000Bot.Services.Extensions;

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

    public async Task<Statistics> Videos(string channelId, CancellationToken cancellationToken)
    {
        var url = $"?part=snippet&channelId={channelId}&order=date&type=video&maxResults=50&key={_configuration.ApiKey}";

        var statistics = await _httpClient.GetAsync<Statistics>(url, _serializerOptions, cancellationToken);

        return statistics;
    }
}