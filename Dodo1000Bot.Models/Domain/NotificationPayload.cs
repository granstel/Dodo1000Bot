using System;
using Dodo1000Bot.Models.GlobalApi;

namespace Dodo1000Bot.Models.Domain
{
    public class NotificationPayload
    {
        public string Text { get; init; }

        public DateTime? HappenedAt { get; init; }

        public CoordinatesModel Coordinates { get; init; }

        public string Address { get; init; }

        public string Name { get; init; }

        public object TemplateArguments { get; set; }

        public override string ToString()
        {
            return $"{HappenedAt:d}{Text}{string.Join(',', TemplateArguments ?? Array.Empty<string>())}".ToUpper().Replace(" ", string.Empty);
        }
    }
}
