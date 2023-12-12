using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Models.PublicApi;
using Dodo1000Bot.Services.Clients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

using AllUnitsDictionary = Dictionary<string, Dictionary<Country, IEnumerable<UnitInfo>>>;

public class PublicApiService : IPublicApiService
{
    private readonly ILogger<PublicApiService> _log;
    private readonly IPublicApiClient _publicApiClient;
    private readonly IGlobalApiService _globalApiService;
    private readonly ISnapshotsRepository _snapshotsRepository;
    private readonly IMemoryCache _memoryCache;

    public PublicApiService(
        ILogger<PublicApiService> log, 
        IPublicApiClient publicApiClient, 
        IGlobalApiService globalApiService, 
        ISnapshotsRepository snapshotsRepository, 
        IMemoryCache memoryCache)
    {
        _log = log;
        _publicApiClient = publicApiClient;
        _globalApiService = globalApiService;
        _snapshotsRepository = snapshotsRepository;
        _memoryCache = memoryCache;
    }

    private string GetUnitInfoOfBrandAtCountrySnapshotName(string brand, string countryCode)
    {
        return $"{nameof(_publicApiClient.UnitInfo)}{brand}{countryCode}";
    }

    public async Task CreateAllUnitsSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCountSnapshot = await _globalApiService.GetUnitsCountSnapshot(cancellationToken);

            if (unitsCountSnapshot is null)
            {
                _log.LogInformation("unitsCountSnapshot is null");
                return;
            }

            var allUnitsInfo = await GetAllUnits(cancellationToken);
            await UpdateAllUnitsSnapshot(allUnitsInfo, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't {methodName}", nameof(CreateAllUnitsSnapshotIfNotExists));
        }
    }

    public async Task<IEnumerable<UnitInfo>> GetUnitInfoOfBrandAtCountrySnapshot(string brand, string countryCode,
        CancellationToken cancellationToken)
    {
        var snapshotName = GetUnitInfoOfBrandAtCountrySnapshotName(brand, countryCode);
        var unitsSnapshot =
            await _snapshotsRepository.Get<IEnumerable<UnitInfo>>(snapshotName, cancellationToken);

        return unitsSnapshot?.Data;
    }

    public async Task UpdateAllUnitsSnapshot(AllUnitsDictionary allUnits, CancellationToken cancellationToken)
    {
        var brands = allUnits.Keys;

        foreach (var brand in brands)
        {
            Dictionary<Country, IEnumerable<UnitInfo>> allUnitsAtBrand = allUnits.GetValueOrDefault(brand);
            var countries = allUnitsAtBrand.Keys;

            foreach (var country in countries)
            {
                var snapshotName = GetUnitInfoOfBrandAtCountrySnapshotName(brand, country.Code);
                var unitsList = allUnitsAtBrand.GetValueOrDefault(country);
                await UpdateSnapshot(snapshotName, unitsList, cancellationToken);
            }
        }
    }
    
    public async Task<AllUnitsDictionary> GetAllUnits(CancellationToken cancellationToken)
    {
        var allUnits = new AllUnitsDictionary();
        
        var brands = await _globalApiService.GetBrands(cancellationToken);
        var brandsNames = brands.Select(b => b.Name).ToList();

        foreach (var brand in brandsNames)
        {
            allUnits.Add(brand, new Dictionary<Country, IEnumerable<UnitInfo>>());
            var countriesOfBrand = await _globalApiService.GetBrandCountries(brand, cancellationToken);

            foreach (var country in countriesOfBrand)
            {
                var countryCode = country.Code;
                var cacheName = GetUnitInfoOfBrandAtCountrySnapshotName(brand, countryCode);

                var unitsInfoOfBrandAtCountry =
                    await _memoryCache.GetOrCreate(cacheName, _ => GetUnitsInfoOfBrandAtCountry(brand, countryCode, cancellationToken));

                allUnits[brand].Add(country, unitsInfoOfBrandAtCountry);
            }
        }

        return allUnits;
    }

    private async Task<IEnumerable<UnitInfo>> GetUnitsInfoOfBrandAtCountry(string brand, string countryCode, CancellationToken cancellationToken)
    {
        var publicApiUnitInfo = await _publicApiClient.UnitInfo(brand, countryCode, cancellationToken);
        var filteredPublicApiUnitInfo = publicApiUnitInfo.Where(u => u.DepartmentState == DepartmentState.Open &&
                                                                     u.State == UnitState.Open &&
                                                                     u.Type == UnitType.Pizzeria).ToArray();

        var departmentsTasks = filteredPublicApiUnitInfo.Select(u => u.DepartmentId).Distinct().Select(id =>
            _publicApiClient.GetDepartmentById(brand, countryCode, id, cancellationToken));

        var publicAPiDepartments = await Task.WhenAll(departmentsTasks);

        var filteredPublicApiDepartmentsIds = publicAPiDepartments.Where(d => d.Type == DepartmentType.Department).Select(d => d.Id).Distinct();
        var filteredPublicApiUnitsByPublicApiDepartments = filteredPublicApiUnitInfo.Where(u => filteredPublicApiDepartmentsIds.Contains(u.DepartmentId));

        return filteredPublicApiUnitsByPublicApiDepartments;
    }

    private async Task UpdateSnapshot<TData>(string snapshotName, TData data, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        var newSnapshot = Snapshot<TData>.Create(snapshotName, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
        _log.LogInformation("Finish UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        
        _memoryCache.Remove(snapshotName);
    }
}