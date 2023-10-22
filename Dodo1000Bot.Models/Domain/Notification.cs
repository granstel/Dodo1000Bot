namespace Dodo1000Bot.Models.Domain
{
    public class Notification
    {
        public Notification(NotificationType type)
        {
            Type = type;
        }
        public int Id { get; set; }

        private NotificationType Type { get; }

        public NotificationPayload Payload { get; set; }

        public string Distinction => $"{Payload}";
    }
}