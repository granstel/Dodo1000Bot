using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient httpClient, string url,
            JsonSerializerOptions serializerOptions, CancellationToken cancellationToken)
        {
            url = url.ToLower();

            var response = await httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();
            var allUnitsString = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<TResult>(allUnitsString, serializerOptions);

            return result;
        }
    }
}