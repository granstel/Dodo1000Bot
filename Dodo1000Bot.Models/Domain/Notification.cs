using System.Text.Json;

namespace Dodo1000Bot.Models.Domain
{
    public class Notification
    {
        public int Id { get; set; }

        public string Payload { get; set; }
        public NotificationPayload DeserializedPayload
        {
            get
            {
                return JsonSerializer.Deserialize<NotificationPayload>(Payload);
            }
        }
    }
}