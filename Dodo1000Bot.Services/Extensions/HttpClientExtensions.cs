using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dodo1000Bot.Services.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient httpClient, string url,
            JsonSerializerSettings serializerSettings, CancellationToken cancellationToken)
        {
            url = url.ToLower();

            var response = await httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();
            var allUnitsString = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonConvert.DeserializeObject<TResult>(allUnitsString, serializerSettings);

            return result;
        }
    }
}