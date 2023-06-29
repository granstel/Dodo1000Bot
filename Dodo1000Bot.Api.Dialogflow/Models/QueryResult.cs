namespace Dodo1000Bot.Api.Dialogflow.Models;

public class QueryResult
{
    public string QueryText { get; set; }

    public string Action { get; set; }

    public Parameters Parameters { get; set; }

    public bool AllRequiredParamsPresent { get; set; }

    public string FulfillmentText { get; set; }

    public FulfillmentMessage[] FulfillmentMessages { get; set; }

    public OutputContexts[] OutputContexts { get; set; }

    public Intent Intent { get; set; }

    public double IntentDetectionConfidence { get; set; }

    public string LanguageCode { get; set; }
}