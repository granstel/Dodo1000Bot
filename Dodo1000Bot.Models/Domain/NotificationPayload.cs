using System;

namespace Dodo1000Bot.Models.Domain
{
    public class NotificationPayload
    {
        public string Text { get; init; }

        public DateTime? HappenedAt { get; init; }

        public override string ToString()
        {
            return $"{HappenedAt:d}:{Text}";
        }
    }
}
