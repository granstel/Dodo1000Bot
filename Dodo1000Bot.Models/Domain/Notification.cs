namespace Dodo1000Bot.Models.Domain
{
    public class Notification
    {
        public Notification(NotificationType type)
        {
            Type = type;
        }
        public int Id { get; set; }

        public NotificationType Type { get; init; }

        public NotificationPayload Payload { get; set; }

        public string Distinction => Payload.ToString();
    }
}