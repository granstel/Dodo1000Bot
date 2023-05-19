using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dodo1000Bot.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Appeal
    {
        NoOfficial,
        Official,
    }
}
