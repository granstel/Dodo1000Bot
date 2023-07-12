namespace Dodo1000Bot.Models.Domain
{
    public class NotificationPayload
    {
        public string Text { get; init; }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}
