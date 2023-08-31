using AutoMapper;
using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Messengers;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Dodo1000Bot.Api.Dialogflow.Tests")]
namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowService : MessengerService<FulfillmentRequest, string>, IDialogflowService
{
    private readonly IDictionary<string, Func<Request, CancellationToken, Task>> _commandsDictionary;

    private readonly IUsersService _usersService;
    private readonly ICustomNotificationsRepository _customNotificationsRepository;
    private readonly INotificationsService _notificationsService;

    public DialogflowService(
        ILogger<DialogflowService> log,
        IConversationService conversationService,
        IMapper mapper,
        IUsersService usersService,
        ICustomNotificationsRepository customNotificationsRepository,
        INotificationsService notificationsService) : base(log, conversationService, mapper)
    {
        _usersService = usersService;
        _customNotificationsRepository = customNotificationsRepository;

        _commandsDictionary = new Dictionary<string, Func<Request, CancellationToken, Task>>
        {
            { "SaveUser", SaveUser },
            { "DeleteUser", DeleteUser },
            { "SaveCustomNotification", SaveCustomNotification },
            { "SendToAdmin", SendToAdmin }
        };
        _notificationsService = notificationsService;
    }

    protected override async Task<Response> ProcessCommand(Request request, CancellationToken cancellationToken)
    {
        var response = new Response { Text = string.Empty };

        if (!_commandsDictionary.TryGetValue(request.Action, out Func<Request, CancellationToken, Task> action))
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
    internal async Task SaveUser(Request request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            MessengerUserId = request.ChatHash,
            MessengerType = request.Source
        };

        await _usersService.SaveAndNotify(user, cancellationToken);
    }

    /// <summary>
    /// Action for key "DeleteUser"
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task DeleteUser(Request request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            MessengerUserId = request.ChatHash,
            MessengerType = request.Source
        };

        await _usersService.Delete(user, cancellationToken);
    }

    /// <summary>
    /// Action for key "SaveCustomNotification"
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task SaveCustomNotification(Request request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = request.Text
            }
        };

        await _customNotificationsRepository.Save(notification, cancellationToken);
    }

    /// <summary>
    /// Action for key "SendToAdmin"
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task SendToAdmin(Request request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = request.Text
            }
        };

        await _notificationsService.SendToAdmin(notification, cancellationToken);
    }
}