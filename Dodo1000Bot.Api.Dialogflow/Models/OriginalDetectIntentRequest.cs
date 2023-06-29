using Dodo1000Bot.Models;

namespace Dodo1000Bot.Api.Dialogflow.Models;

public class OriginalDetectIntentRequest
{
    public Source Source { get; set; }

    public Payload Payload { get; set; }
}