using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Models.PublicApi;
using Dodo1000Bot.Services.Clients;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

using AllUnitsDictionary = Dictionary<Brands, Dictionary<UnitCountModel, IEnumerable<UnitInfo>>>;

public class SnapshotsService : ISnapshotsService
{
    private readonly ILogger<SnapshotsService> _log;
    private readonly IGlobalApiClient _globalApiClient;
    private readonly IPublicApiClient _publicApiClient;
    private readonly ISnapshotsRepository _snapshotsRepository;

    public SnapshotsService(
        ILogger<SnapshotsService> log, 
        IGlobalApiClient globalApiClient, 
        IPublicApiClient publicApiClient, 
        ISnapshotsRepository snapshotsRepository)
    {
        _log = log;
        _globalApiClient = globalApiClient;
        _publicApiClient = publicApiClient;
        _snapshotsRepository = snapshotsRepository;
    }

    private async Task UpdateSnapshot<TData>(string snapshotName, TData data, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        var newSnapshot = Snapshot<TData>.Create(snapshotName, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
        _log.LogInformation("Finish UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
    }

    private string GetUnitsOfBrandAtCountrySnapshotName(Brands brand, int countryId)
    {
        return $"{nameof(_globalApiClient.UnitsOfBrandAtCountry)}{brand}{countryId}";
    }

    private string GetUnitInfoOfBrandAtCountrySnapshotName(Brands brand, int countryId)
    {
        return $"{nameof(_publicApiClient.UnitInfo)}{brand}{countryId}";
    }


    public async Task CreateAllUnitsSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCountSnapshot = await GetUnitsCountSnapshot(cancellationToken);

            if (unitsCountSnapshot is null)
            {
                _log.LogInformation("unitsCountSnapshot is null");
                return;
            }

            var allUnitsInfo = await GetAllUnits(unitsCountSnapshot, cancellationToken);
            await UpdateAllUnitsSnapshot(allUnitsInfo, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't {methodName}", nameof(CreateUnitsCountSnapshotIfNotExists));
        }
    }

    public async Task<IEnumerable<UnitInfo>> GetUnitInfoOfBrandAtCountrySnapshot(Brands brand, int countryId, CancellationToken cancellationToken)
    {
        var snapshotName = GetUnitInfoOfBrandAtCountrySnapshotName(brand, countryId);
        var unitsSnapshot =
            await _snapshotsRepository.Get<IEnumerable<UnitInfo>>(snapshotName, cancellationToken);

        return unitsSnapshot?.Data;
    }
    
    public async Task CreateUnitsSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCountSnapshot = await GetUnitsCountSnapshot(cancellationToken);

            if (unitsCountSnapshot is null)
            {
                _log.LogInformation("unitsCountSnapshot is null");
                return;
            }
            
            List<Brands> brands = unitsCountSnapshot.Brands.Select(b => b.Brand).ToList();

            foreach (var brand in brands)
            {
                var totalUnitCountListModel = unitsCountSnapshot
                    .Brands.First(b => b.Brand == brand);

                foreach (var country in totalUnitCountListModel.Countries)
                {
                    var countryId = country.CountryId;
                    var snapshotName = GetUnitsOfBrandAtCountrySnapshotName(brand, countryId);
                    var unitsSnapshot = 
                        await _snapshotsRepository.Get<BrandData<UnitListModel>>(snapshotName, cancellationToken);

                    if (unitsSnapshot?.Data is not null)
                    {
                        _log.LogInformation("unitsSnapshot at {brand} in {country} is not null", brand, countryId);
                        return;
                    }

                    BrandData<UnitListModel> unitsAtCountry = await _globalApiClient.UnitsOfBrandAtCountry(brand, countryId, cancellationToken);
                    await UpdateSnapshot(snapshotName, unitsAtCountry, cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't {methodName}", nameof(CreateUnitsCountSnapshotIfNotExists));
        }
    }

    public async Task UpdateAllUnitsSnapshot(AllUnitsDictionary allUnits, CancellationToken cancellationToken)
    {
        var brands = allUnits.Keys;

        foreach (var brand in brands)
        {
            Dictionary<UnitCountModel, IEnumerable<UnitInfo>> allUnitsAtBrand = allUnits.GetValueOrDefault(brand);
            var countries = allUnitsAtBrand.Keys;

            foreach (var country in countries)
            {
                var snapshotName = GetUnitInfoOfBrandAtCountrySnapshotName(brand, country.CountryId);
                var unitsList = allUnitsAtBrand.GetValueOrDefault(country);
                await UpdateSnapshot(snapshotName, unitsList, cancellationToken);
            }
        }
    }
    
    private async Task<AllUnitsDictionary> GetAllUnits(BrandListTotalUnitCountListModel globalApiUnitsCount, CancellationToken cancellationToken)
    {
        var allUnits = new AllUnitsDictionary();
        var brands = globalApiUnitsCount.Brands.Select(b => b.Brand).ToList();

        foreach (var brand in brands)
        {
            allUnits.Add(brand, new Dictionary<UnitCountModel, IEnumerable<UnitInfo>>());
            var countriesOfBrand = globalApiUnitsCount
                .Brands.First(b => b.Brand == brand).Countries;

            foreach (var country in countriesOfBrand)
            {
                var publicApiUnitInfo = await _publicApiClient.UnitInfo(brand, country.CountryCode, cancellationToken);
                var filteredPublicApiUnitInfo = publicApiUnitInfo.Where(u => u.DepartmentState == DepartmentState.Open &&
                                                                             u.State == UnitState.Open &&
                                                                             u.Type == UnitType.Pizzeria).ToArray();

                var departmentsTasks = filteredPublicApiUnitInfo.Select(u => u.DepartmentId).Distinct().Select(id =>
                    _publicApiClient.GetDepartmentById(brand, country.CountryCode, id, cancellationToken));

                var publicAPiDepartments = await Task.WhenAll(departmentsTasks);

                var filteredPublicApiDepartmentsIds = publicAPiDepartments.Where(d => d.Type == DepartmentType.Department).Select(d => d.Id).Distinct();
                var filteredPublicApiUnitsByPublicApiDepartments = filteredPublicApiUnitInfo.Where(u => filteredPublicApiDepartmentsIds.Contains(u.DepartmentId));

                allUnits[brand].Add(country, filteredPublicApiUnitsByPublicApiDepartments);
            }
        }

        return allUnits;
    }
}