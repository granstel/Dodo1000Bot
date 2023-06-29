using System.Threading.Tasks;
using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Messengers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowController : MessengerController<FulfillmentRequest, string>
{
    private readonly IDialogflowService _dialogflowService;

    public DialogflowController(ILogger<DialogflowController> log, IDialogflowService dialogflowService, DialogflowConfiguration configuration)
        : base(log, dialogflowService, configuration)
    {
        _dialogflowService = dialogflowService;
    }
}