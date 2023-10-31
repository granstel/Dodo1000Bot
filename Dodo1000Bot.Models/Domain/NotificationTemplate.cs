namespace Dodo1000Bot.Models.Domain
{
    public class NotificationTemplate
    {
        public int Id { get; set; }

        public NotificationType NotificationType { get; set; }

        public Source MessengerType { get; set; }

        public string LanguageCode { get; set; }

        public string Template { get; set; }
    }
}
