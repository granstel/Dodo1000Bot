using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Services;
using Dodo1000Bot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Api.Jobs;

public class UnitsJob : RepeatableJob
{
    private readonly IUnitsService _unitsService;

    public UnitsJob(ILogger<UnitsJob> log,
        IUnitsService unitsService,
        UnitsConfiguration unitsConfiguration) : base(log, unitsConfiguration.RefreshEveryTime)
    {
        _unitsService = unitsService;
    }

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        await _unitsService.CheckAndNotify(cancellationToken);
    }
}