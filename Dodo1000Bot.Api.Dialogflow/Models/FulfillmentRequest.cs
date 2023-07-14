namespace Dodo1000Bot.Api.Dialogflow.Models;

public class FulfillmentRequest
{
    public string ResponseId { get; set; }

    public QueryResult QueryResult { get; set; }

    public OriginalDetectIntentRequest OriginalDetectIntentRequest { get; set; }

    public string Session { get; set; }
}

