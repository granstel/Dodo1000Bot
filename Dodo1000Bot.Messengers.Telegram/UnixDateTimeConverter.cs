using System;
using System.Text.Json;

namespace Dodo1000Bot.Messengers.Telegram;

public class UnixDateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var number = reader.GetInt64();
            var offset = DateTimeOffset.FromUnixTimeSeconds(number);
            return offset.DateTime;
        }

        return new DateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
    }
}