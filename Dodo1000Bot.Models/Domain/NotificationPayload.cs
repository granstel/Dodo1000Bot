namespace Dodo1000Bot.Models.Domain
{
    public class NotificationPayload
    {
        public string Text { get; init; }

        public override string ToString()
        {
            return Text;
        }
    }
}
