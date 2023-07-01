using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Messengers;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowService : MessengerService<FulfillmentRequest, string>, IDialogflowService
{
    private readonly IUsersRepository _usersRepository;

    public DialogflowService(
        ILogger<DialogflowService> log,
        IConversationService conversationService,
        IUsersRepository usersRepository,
        IMapper mapper) : base(log, conversationService, mapper)
    {
        _usersRepository = usersRepository;
    }

    public override async Task<string> ProcessIncomingAsync(FulfillmentRequest input, CancellationToken ct)
    {
        var user = new User
        {
            MessengerUserId = input.OriginalDetectIntentRequest.Payload.Data.Chat.Id,
            MessengerType = input.OriginalDetectIntentRequest.Source
        };

        try
        {
            await _usersRepository.SaveUser(user, ct);
        }
        catch (Exception e)
        {
            Log.LogError(e, "Can't save user");
        }

        return string.Empty;
    }
}