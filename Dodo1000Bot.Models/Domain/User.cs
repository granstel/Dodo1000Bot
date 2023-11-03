namespace Dodo1000Bot.Models.Domain
{
    public class User
    {
        public int Id { get; set; }

        public string MessengerUserId { get; set; }

        public Source MessengerType { get; set; }

        public string LanguageCode { get; set; }
    }
}
