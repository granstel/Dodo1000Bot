using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class GlobalApiService : IGlobalApiService, IAsyncDisposable
{
    private readonly ILogger<GlobalApiService> _log;
    private readonly IGlobalApiClient _globalApiClient;
    private readonly ISnapshotsRepository _snapshotsRepository;
    private readonly IMemoryCache _memoryCache;

    public GlobalApiService(
        ILogger<GlobalApiService> log, 
        IGlobalApiClient globalApiClient, 
        ISnapshotsRepository snapshotsRepository, 
        IMemoryCache memoryCache)
    {
        _log = log;
        _globalApiClient = globalApiClient;
        _snapshotsRepository = snapshotsRepository;
        _memoryCache = memoryCache;
    }

    public async Task CreateUnitsCountSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCountSnapshot = await UnitsCountSnapshot(cancellationToken);

            if (unitsCountSnapshot is not null)
            {
                _log.LogInformation("unitsCountSnapshot is not null");
                return;
            }

            await UpdateUnitsCountSnapshot(cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't {methodName}", nameof(CreateUnitsCountSnapshotIfNotExists));
        }
    }

    public async Task<BrandListTotalUnitCountListModel> UnitsCount(CancellationToken cancellationToken)
    {
        var cacheName = nameof(_globalApiClient.UnitsCount);
        var unitsCount = await _memoryCache.GetOrCreate(cacheName, _ => _globalApiClient.UnitsCount(cancellationToken));
        return unitsCount;
    }

    public async Task<BrandListTotalUnitCountListModel> UnitsCountSnapshot(CancellationToken cancellationToken)
    {
        var snapshotName = nameof(_globalApiClient.UnitsCount);
        var unitsCountSnapshot =
            await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(snapshotName, cancellationToken);

        return unitsCountSnapshot?.Data;
    }

    public async Task UpdateUnitsCountSnapshot(CancellationToken cancellationToken)
    {
        var unitsCount = await UnitsCount(cancellationToken);

        var snapshotName = nameof(_globalApiClient.UnitsCount);
        await UpdateSnapshot(snapshotName, unitsCount, cancellationToken);
    }

    public async Task<IEnumerable<Brand>> GetBrands(CancellationToken cancellationToken)
    {
        var cacheName = nameof(_globalApiClient.GetBrands);
        var brands = await _memoryCache.GetOrCreate(cacheName, _ => _globalApiClient.GetBrands(cancellationToken));

        return brands;
    }

    public async Task<IEnumerable<Country>> GetBrandCountries(string brand, CancellationToken cancellationToken)
    {
        var cacheName = $"{nameof(_globalApiClient.GetBrandCountries)}{brand}";
        var brandCountries = await _memoryCache.GetOrCreate(cacheName, _ => _globalApiClient.GetBrandCountries(brand, cancellationToken));

        return brandCountries;
    }

    private async Task UpdateSnapshot<TData>(string snapshotName, TData data, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        var newSnapshot = Snapshot<TData>.Create(snapshotName, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
        _log.LogInformation("Finish UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        
        _memoryCache.Remove(snapshotName);
    }

    public async ValueTask DisposeAsync()
    {
        await UpdateUnitsCountSnapshot(default);
    }
}