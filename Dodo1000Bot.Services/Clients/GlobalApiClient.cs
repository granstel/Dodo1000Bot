using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.GlobalApi;

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
            const string url = "units/count";

            var unitsCount = await _httpClient.GetFromJsonAsync<BrandListTotalUnitCountListModel>(url, _serializerOptions, cancellationToken);

            return unitsCount;
        }

        public async Task<BrandData<UnitListModel>> UnitsOfBrandAtCountry(Brands brand, int countryId, CancellationToken cancellationToken)
        {
            var url = $"{brand}/units/all/{countryId}";

            var unitsCount = await _httpClient.GetFromJsonAsync<BrandData<UnitListModel>>(url, _serializerOptions, cancellationToken);

            return unitsCount;
        }
    }
}