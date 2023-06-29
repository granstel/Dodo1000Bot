using System;
using System.Threading.Tasks;
using AutoMapper;
using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Messengers;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowService : MessengerService<FulfillmentRequest, string>, IDialogflowService
{
    public DialogflowService(
        ILogger<DialogflowService> log,
        IConversationService conversationService,
        IMapper mapper) : base(log, conversationService, mapper)
    {
    }

    public Task<string> ProcessIncomingAsync(FulfillmentRequest input)
    {
        throw new NotImplementedException();
    }
}