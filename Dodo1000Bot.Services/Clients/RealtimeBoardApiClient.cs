using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services.Extensions;

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

        public async Task<int> OrdersPerMinute(CancellationToken cancellationToken)
        {
            const string url = "statistics/OrdersCountPerMinute";

            var ordersPerMinute = await _httpClient.GetAsync<int>(url, _serializerOptions, cancellationToken);

            return ordersPerMinute;
        }
    }
}