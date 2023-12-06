using System;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class GlobalApiService : IGlobalApiService
{
    private readonly ILogger<GlobalApiService> _log;
    private readonly IGlobalApiClient _globalApiClient;
    private readonly ISnapshotsRepository _snapshotsRepository;

    public GlobalApiService(
        ILogger<GlobalApiService> log, 
        IGlobalApiClient globalApiClient, 
        ISnapshotsRepository snapshotsRepository)
    {
        _log = log;
        _globalApiClient = globalApiClient;
        _snapshotsRepository = snapshotsRepository;
    }
    
    public async Task CreateUnitsCountSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCountSnapshot = await GetUnitsCountSnapshot(cancellationToken);

            if (unitsCountSnapshot is not null)
            {
                _log.LogInformation("unitsCountSnapshot is not null");
                return;
            }

            await GetUnitsCount(cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't {methodName}", nameof(CreateUnitsCountSnapshotIfNotExists));
        }
    }

    public async Task<BrandListTotalUnitCountListModel> GetUnitsCount(CancellationToken cancellationToken)
    {
        var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);

        return unitsCount;
    }

    public async Task<BrandListTotalUnitCountListModel> GetUnitsCountSnapshot(CancellationToken cancellationToken)
    {
        var snapshotName = nameof(_globalApiClient.UnitsCount);
        var unitsCountSnapshot =
            await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(snapshotName, cancellationToken);

        return unitsCountSnapshot?.Data;
    }

    public async Task UpdateUnitsCountSnapshot(CancellationToken cancellationToken)
    {
        var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);

        var snapshotName = nameof(_globalApiClient.UnitsCount);
        await UpdateSnapshot(snapshotName, unitsCount, cancellationToken);
    }

    private async Task UpdateSnapshot<TData>(string snapshotName, TData data, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        var newSnapshot = Snapshot<TData>.Create(snapshotName, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
        _log.LogInformation("Finish UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
    }
}