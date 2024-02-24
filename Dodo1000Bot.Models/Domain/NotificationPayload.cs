using System;

namespace Dodo1000Bot.Models.Domain
{
    public class NotificationPayload
    {
        public string Text { get; init; }

        public DateTime? HappenedAt { get; init; }

        public Coordinates Coordinates { get; set; }

        public string Address { get; init; }

        public string Name { get; init; }

        public override string ToString()
        {
            return $"{HappenedAt:d}{Text}".ToUpper().Replace(" ", string.Empty);
        }
    }
}
