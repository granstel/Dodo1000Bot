using System.Collections.Generic;

namespace Dodo1000Bot.Services
{
    public static class Constants
    {
        public static readonly Dictionary<string, string> TelegramFlags = new()
        {
            {"ae", "🇦🇪"},
            {"am", "🇦🇲"},
            {"az", "🇦🇿"},
            {"bg", "🇧🇬"},
            {"by", "🇧🇾"},
            {"cn", "🇨🇳"},
            {"cy", "🇨🇾"},
            {"de", "🇩🇪"},
            {"ee", "🇪🇪"},
            {"gb", "🇬🇧"},
            {"ge", "🇬🇪"},
            {"hr", "🇭🇷"},
            {"id", "🇮🇩"},
            {"kg", "🇰🇬"},
            {"kz", "🇰🇿"},
            {"lt", "🇱🇹"},
            {"ng", "🇳🇬"},
            {"pl", "🇵🇱"},
            {"ro", "🇷🇴"},
            {"rs", "🇷🇸"},
            {"ru", "🇷🇺"},
            {"si", "🇸🇮"},
            {"tj", "🇹🇯"},
            {"tr", "🇹🇷"},
            {"uz", "🇺🇿"},
            {"vn", "🇻🇳"},
        };

        public static readonly Dictionary<string, string> BrandsEmoji = new()
        {
            { "dodopizza", "🍕" },
            { "drinkit", "☕" },
            { "doner42", "🌯" },
        };
    }
}
