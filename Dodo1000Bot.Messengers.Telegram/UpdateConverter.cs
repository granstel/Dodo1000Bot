using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot.Types;

namespace Dodo1000Bot.Messengers.Telegram;

public class UpdateConverter : JsonConverter<Update>
{
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateConverter(JsonSerializerOptions jsonOptions)
    {
        _jsonOptions = jsonOptions;
    }
    public override Update Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var updateString = reader.GetString();
        var update = JsonSerializer.Deserialize(updateString!, typeToConvert, _jsonOptions);
        return update as Update;
    }

    public override void Write(Utf8JsonWriter writer, Update value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}