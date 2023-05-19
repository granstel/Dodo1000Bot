using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services
{
    public static class InternalLoggerFactory
    {
        public static ILoggerFactory Factory { get; set; }

        public static ILogger<T> CreateLogger<T>() => Factory?.CreateLogger<T>();

        public static ILogger CreateLogger(string categoryName) => Factory?.CreateLogger(categoryName);
    }
}