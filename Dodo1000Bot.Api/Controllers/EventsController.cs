using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dodo1000Bot.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController: Controller
{
    private const string TokenParameter = "token";

    private readonly ManagementConfiguration _configuration;
    private readonly INotificationsService _notificationsService;

    public EventsController(ManagementConfiguration configuration, INotificationsService notificationsService)
    {
        _configuration = configuration;
        _notificationsService = notificationsService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var isValid = IsValidRequest(context);

        if (!isValid)
        {
            context.Result = NotFound();
        }
    }

    [HttpGet]
    public string GetInfo()
    {
        return $"{DateTime.Now:F}";
    }

    [HttpPut("{token?}")]
    public async Task<IActionResult> Put([FromBody]Notification notification, string token, CancellationToken cancellationToken)
    {
        await _notificationsService.Save(notification, cancellationToken);

        return Ok();
    }

    private bool IsValidRequest(ActionExecutingContext context)
    {
        var actionHasTokenParameter = context.ActionDescriptor.Parameters.Any(p => string.Equals(p.Name, TokenParameter));

        if (string.IsNullOrEmpty(_configuration.IncomingToken) || !actionHasTokenParameter)
        {
            return true;
        }

        context.ActionArguments.TryGetValue(TokenParameter, out object value);

        var token = value as string;

        return string.Equals(_configuration.IncomingToken, token, StringComparison.InvariantCultureIgnoreCase);
    }
}