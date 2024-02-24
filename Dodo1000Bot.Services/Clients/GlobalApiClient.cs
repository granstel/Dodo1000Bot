using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
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
            const string url = "units/count";

            var unitsCount = await _httpClient.GetAsync<BrandListTotalUnitCountListModel>(url, _serializerOptions, cancellationToken);

            return unitsCount;
        }

        public async Task<BrandData<UnitListModel>> UnitsOfBrandAtCountry(Brands brand, int countryId, CancellationToken cancellationToken)
        {
            var url = $"{brand}/units/all/{countryId}";

            var unitsCount = await _httpClient.GetAsync<BrandData<UnitListModel>>(url, _serializerOptions, cancellationToken);

            return unitsCount;
        }

        public async Task<IEnumerable<Brand>> GetBrands(CancellationToken cancellationToken)
        {
            var url = "brands";

            var brands = await _httpClient.GetAsync<IEnumerable<Brand>>(url, _serializerOptions, cancellationToken);

            return brands;
        }

        public async Task<IEnumerable<Country>> GetBrandCountries(string brand, CancellationToken cancellationToken)
        {
            var url = $"{brand}/countries";

            var brandCountries = await _httpClient.GetAsync<IEnumerable<Country>>(url, _serializerOptions, cancellationToken);

            return brandCountries;
        }
    }
}