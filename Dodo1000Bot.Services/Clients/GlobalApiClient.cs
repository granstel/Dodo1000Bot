using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Services.Extensions;

namespace Dodo1000Bot.Services.Clients
{
    public class GlobalApiClient : IGlobalApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions  _serializerOptions;

        public GlobalApiClient(HttpClient httpClient, JsonSerializerOptions serializerOptions)
        {
            _httpClient = httpClient;
            _serializerOptions = serializerOptions;
        }

        public async Task<BrandListTotalUnitCountListModel> UnitsCount(CancellationToken cancellationToken)
        {
            var url = "units/count";

            var unitsCount = await _httpClient.GetAsync<BrandListTotalUnitCountListModel>(url, _serializerOptions, cancellationToken);

            return unitsCount;
        }
    }
}