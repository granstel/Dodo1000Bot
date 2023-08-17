using Dodo1000Bot.Models;

namespace Dodo1000Bot.Api.Dialogflow.Models;

public class Data
{
    public From From { get; set; }

    public Chat Chat { get; set; }

    public string MessageId { get; set; }

    public string Date { get; set; }

    public string Text { get; set; }

    public FormattingEntity[] Entities { get; set; }
}