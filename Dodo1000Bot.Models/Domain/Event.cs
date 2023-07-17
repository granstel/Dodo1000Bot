namespace Dodo1000Bot.Models.Domain
{
    public class Event
    {
        public int Id { get; set; }

        public EventPayload Payload { get; set; }

        public string Distinction => Payload.ToString().ToUpper()
            .Replace(" ", string.Empty);
    }
}