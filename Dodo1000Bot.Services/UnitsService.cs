using System.Threading;
using System.Threading.Tasks;

namespace Dodo1000Bot.Services;

public class UnitsService : IUnitsService
{
    private IGlobalApiClient _globalApiClient;

    public UnitsService(IGlobalApiClient globalApiClient)
    {
        _globalApiClient = globalApiClient;
    }

    public async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);
    }
}