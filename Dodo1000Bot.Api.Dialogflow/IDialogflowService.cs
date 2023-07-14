using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Services;

namespace Dodo1000Bot.Api.Dialogflow;

public interface IDialogflowService : IMessengerService<FulfillmentRequest, string>
{
}