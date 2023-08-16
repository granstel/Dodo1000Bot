using AutoMapper;
using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Messengers;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowService : MessengerService<FulfillmentRequest, string>, IDialogflowService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IDictionary<string, Func<Request, CancellationToken, Task>> _commandsDictionary;

    public DialogflowService(
        ILogger<DialogflowService> log,
        IConversationService conversationService,
        IMapper mapper,
        IUsersRepository usersRepository) : base(log, conversationService, mapper)
    {
        _usersRepository = usersRepository;

        _commandsDictionary = new Dictionary<string, Func<Request, CancellationToken, Task>>
        {
            { "SaveUser", SaveUser },
            { "DeleteUser", DeleteUser }
        };
    }

    protected override async Task<Response> ProcessCommand(Request request, CancellationToken cancellationToken)
    {
        var response = new Response { Text = string.Empty };

        if (!_commandsDictionary.TryGetValue(request.Text, out Func<Request, CancellationToken, Task> action))
        {
            return response;
        }

        try
        {
            await action(request, cancellationToken);
        }
        catch (Exception e)
        {
            Log.LogError(e, "Can't {MethodName}", action.Method.Name);
        }
        
        return response;
    }

    /// <summary>
    /// Action for key "SaveUser"
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task SaveUser(Request request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            MessengerUserId = request.UserHash,
            MessengerType = request.Source
        };

        await _usersRepository.SaveUser(user, cancellationToken);
    }

    /// <summary>
    /// Action for key "DeleteUser"
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DeleteUser(Request request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            MessengerUserId = request.UserHash,
            MessengerType = request.Source
        };

        await _usersRepository.Delete(user, cancellationToken);
    }
}