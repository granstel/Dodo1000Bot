using AutoMapper;
using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Messengers;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowService : MessengerService<FulfillmentRequest, string>, IDialogflowService
{
    private readonly IUsersRepository _usersRepository;

    public DialogflowService(
        ILogger<DialogflowService> log,
        IConversationService conversationService,
        IMapper mapper,
        IUsersRepository usersRepository) : base(log, conversationService, mapper)
    {
        _usersRepository = usersRepository;
    }

    protected override async Task<Response> ProcessCommand(Request request, CancellationToken cancellationToken)
    {
        await SaveUser(request.UserHash, request.Source, cancellationToken);

        return new Response { Text = string.Empty };
    }

    private async Task SaveUser(string userHash, Source source, CancellationToken cancellationToken)
    {
        var user = new User
        {
            MessengerUserId = userHash,
            MessengerType = source
        };

        try
        {
            await _usersRepository.SaveUser(user, cancellationToken);
        }
        catch (Exception e)
        {
            Log.LogError(e, "Can't save user");
        }
    }
}