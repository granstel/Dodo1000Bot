using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.RealtimeBoard;

namespace Dodo1000Bot.Services.Clients
{
    public class RealtimeBoardApiClient : IRealtimeBoardApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions  _serializerOptions;

        public RealtimeBoardApiClient(HttpClient httpClient, JsonSerializerOptions serializerOptions)
        {
            _httpClient = httpClient;
            _serializerOptions = serializerOptions;
        }

        public async Task<Statistics> Statistics(CancellationToken cancellationToken)
        {
            const string url = "statistics";

            var statistics = await _httpClient.GetFromJsonAsync<Statistics>(url, _serializerOptions, cancellationToken);

            return statistics;
        }
    }
}