namespace Dodo1000Bot.Models.Domain
{
    public class Notification
    {
        public int Id { get; set; }

        public NotificationPayload Payload { get; set; }

        public int Distinction => Payload.GetHashCode();
    }
}