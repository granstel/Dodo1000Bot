using System.Text.Json.Serialization;

namespace Dodo1000Bot.Models;

public enum FormattingTypes
{
    /// <summary>
    /// A mentioned <see cref="User"/>
    /// </summary>
    [JsonPropertyName("mention")]
    Mention = 1,

    /// <summary>
    /// A searchable Hashtag
    /// </summary>
    [JsonPropertyName("hashtag")]
    Hashtag,

    /// <summary>
    /// A Bot command
    /// </summary>
    [JsonPropertyName("bot_command")]
    BotCommand,

    /// <summary>
    /// An URL
    /// </summary>
    [JsonPropertyName("url")]
    Url,

    /// <summary>
    /// An email
    /// </summary>
    [JsonPropertyName("email")]
    Email,

    /// <summary>
    /// Bold text
    /// </summary>
    [JsonPropertyName("bold")]
    Bold,

    /// <summary>
    /// Italic text
    /// </summary>
    [JsonPropertyName("italic")]
    Italic,

    /// <summary>
    /// Monowidth string
    /// </summary>
    [JsonPropertyName("code")]
    Code,

    /// <summary>
    /// Monowidth block
    /// </summary>
    [JsonPropertyName("pre")]
    Pre,

    /// <summary>
    /// Clickable text URLs
    /// </summary>
    [JsonPropertyName("text_link")]
    TextLink,

    /// <summary>
    /// Mentions for a <see cref="User"/> without <see cref="User.Username"/>
    /// </summary>
    [JsonPropertyName("text_mention")]
    TextMention,

    /// <summary>
    /// Phone number
    /// </summary>
    [JsonPropertyName("phone_number")]
    PhoneNumber,

    /// <summary>
    /// A cashtag (e.g. $EUR, $USD) - $ followed by the short currency code
    /// </summary>
    [JsonPropertyName("cashtag")]
    Cashtag,

    /// <summary>
    /// Underlined text
    /// </summary>
    [JsonPropertyName("underline")]
    Underline,

    /// <summary>
    /// Strikethrough text
    /// </summary>
    [JsonPropertyName("strikethrough")]
    Strikethrough,

    /// <summary>
    /// Spoiler message
    /// </summary>
    [JsonPropertyName("spoiler")]
    Spoiler,

    /// <summary>
    /// Inline custom emoji stickers
    /// </summary>
    [JsonPropertyName("custom_emoji")]
    CustomEmoji,
}