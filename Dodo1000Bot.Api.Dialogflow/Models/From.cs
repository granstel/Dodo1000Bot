using System.Text.Json.Serialization;

namespace Dodo1000Bot.Api.Dialogflow.Models;

public class From
{
    public string Id { get; set; }

    public string Username { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("language_code")]
    public string LanguageCode { get; set; }
}