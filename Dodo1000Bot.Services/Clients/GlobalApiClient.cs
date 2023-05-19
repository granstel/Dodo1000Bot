using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Services.Extensions;
using Newtonsoft.Json;

namespace Dodo1000Bot.Services.Clients
{
    public class GlobalApiClient : IGlobalApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public GlobalApiClient(HttpClient httpClient, JsonSerializerSettings jsonSerializerSettings)
        {
            _httpClient = httpClient;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task<BrandListTotalUnitCountListModel> UnitsCount(CancellationToken cancellationToken)
        {
            var url = "units/count";

            var unitsCount = await _httpClient.GetAsync<BrandListTotalUnitCountListModel>(url, _jsonSerializerSettings, cancellationToken);

            return unitsCount;
        }
    }
}