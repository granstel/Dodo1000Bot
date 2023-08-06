using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Services.Interfaces;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Dodo1000Bot.Services.Tests")]

namespace Dodo1000Bot.Services;

public class UnitsService : CheckAndNotifyService
{
    private readonly ILogger<UnitsService> _log;
    private readonly IGlobalApiClient _globalApiClient;
    private readonly INotificationsService _notificationsService;
    private readonly ISnapshotsRepository _snapshotsRepository;

    public UnitsService(
        ILogger<UnitsService> log, 
        IGlobalApiClient globalApiClient, 
        INotificationsService notificationsService, 
        ISnapshotsRepository snapshotsRepository)
    {
        _log = log;
        _globalApiClient = globalApiClient;
        _notificationsService = notificationsService;
        _snapshotsRepository = snapshotsRepository;
    }

    public override async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);

            await AboutTotalOverall(unitsCount, cancellationToken);
            await AboutTotalAtBrands(unitsCount, cancellationToken);
            await AboutTotalAtCountries(unitsCount, cancellationToken);

            var snapshotName = nameof(_globalApiClient.UnitsCount);
            var unitsCountSnapshot = 
                await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(snapshotName, cancellationToken);

            await AboutNewCountries(unitsCount, unitsCountSnapshot.Data, cancellationToken);
            await AboutNewUnits(unitsCount, unitsCountSnapshot.Data, cancellationToken);

            await UpdateSnapshot(snapshotName, unitsCount, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't check and notify units count");
        }
    }

    private async Task AboutTotalOverall(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalOverall = unitsCount.Brands.Sum(b => b.Total);

        if (!CheckRemainder1000(totalOverall))
        {
            return;
        }

        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text = $"There is {totalOverall} units!"
            }
        };

        await _notificationsService.Save(notification, cancellationToken);
    }

    private async Task AboutTotalAtBrands(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalAtBrands = unitsCount.Brands.ToDictionary(b => b.Brand, b => b.Total);

        foreach (var totalAtBrand in totalAtBrands)
        {
            if (!CheckRemainder1000(totalAtBrand.Value))
            {
                continue;
            }

            var notification = new Notification
            {
                Payload = new NotificationPayload
                {
                    Text = $"There is {totalAtBrand.Value} units of {totalAtBrand.Key} brand!"
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
    }

    internal async Task AboutTotalAtCountries(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalAtBrandAtCountries = unitsCount.Brands
            .ToDictionary(b => b.Brand, b => b.Countries.ToDictionary(c => c.CountryName, c => c.PizzeriaCount));

        foreach (var totalAtBrandAtCountry in totalAtBrandAtCountries)
        {
            var brand = totalAtBrandAtCountry.Key;
            foreach (var totalAtCountry in totalAtBrandAtCountry.Value)
            {
                await CheckAndNotify1000(totalAtCountry, brand, cancellationToken);
            }
        }
    }

    internal async Task AboutNewCountries(
        BrandListTotalUnitCountListModel unitsCount, 
        BrandListTotalUnitCountListModel unitsCountSnapshot, 
        CancellationToken cancellationToken)
    {
        if (unitsCountSnapshot is null)
        {
            return;
        }

        Dictionary<Brands, List<string>> countriesAtBrand = GetCountriesAtBrands(unitsCount.Brands);
        Dictionary<Brands, List<string>> countriesAtBrandSnapshot = GetCountriesAtBrands(unitsCountSnapshot.Brands);

        foreach (var brand in countriesAtBrand.Keys)
        {
            List<string> countries = countriesAtBrand.GetValueOrDefault(brand);
            List<string> countriesSnapshot = GetValueOrDefault(countriesAtBrandSnapshot, brand, new List<string>());

            if (countries.Count == countriesSnapshot.Count)
            {
                return;
            }

            var difference = countries.Except(countriesSnapshot);

            await CheckDifferenceAndNotify(brand, difference, cancellationToken);
        }
    }

    /// <summary>
    /// F**k you, cyclomatic complexity!
    /// </summary>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    private TValue GetValueOrDefault<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key,
        TValue defaultValue)
    {
        return source.GetValueOrDefault(key) ?? defaultValue;
    }

    private Dictionary<Brands,List<string>> GetCountriesAtBrands(IEnumerable<BrandTotalUnitCountListModel> unitsCountBrands)
    {
        return unitsCountBrands.ToDictionary(b => b.Brand, b => b.Countries.Select(c => c.CountryName).ToList());
    }

    internal async Task AboutNewUnits(BrandListTotalUnitCountListModel unitsCount, BrandListTotalUnitCountListModel unitsCountSnapshot, CancellationToken cancellationToken)
    {
        List<Brands> brands = unitsCount.Brands.Select(b => b.Brand).ToList();

        foreach (var brand in brands)
        {
            var totalUnitCountListModel = unitsCount
                .Brands.First(b => b.Brand == brand);

            foreach (var country in totalUnitCountListModel.Countries)
            {
                await CheckUnitsCountAtCountryAndNotify(brand, country.CountryId, country.PizzeriaCount, 
                    unitsCountSnapshot, cancellationToken);
            }
        }
    }

    private async Task CheckUnitsCountAtCountryAndNotify(Brands brand, int countryId, int unitsCount,
        BrandListTotalUnitCountListModel unitsCountSnapshot, CancellationToken cancellationToken)
    {
        if (unitsCountSnapshot is null)
        {
            return;
        }

        var unitsCountAtCountrySnapshot = unitsCountSnapshot
            .Brands.FirstOrDefault(b => b.Brand == brand)?
            .Countries.Where(c => c.CountryId == countryId)
            .Select(c => c.PizzeriaCount).FirstOrDefault();

        if (unitsCount == unitsCountAtCountrySnapshot)
        {
            await UpdateUnitsOfBrandAtCountrySnapshot(brand, countryId, cancellationToken);
            return;
        }

        await CheckUnitsOfBrandAtCountryAndNotify(brand, countryId, cancellationToken);
    }

    internal async Task CheckUnitsOfBrandAtCountryAndNotify(Brands brand, int countryId, CancellationToken cancellationToken)
    {
        BrandData<UnitListModel> unitsAtCountry = await _globalApiClient.UnitsOfBrandAtCountry(brand, countryId, cancellationToken);

        var snapshotName = GetUnitsOfBrandAtCountrySnapshotName(brand, countryId);
        var unitsSnapshot = 
            await _snapshotsRepository.Get<BrandData<UnitListModel>>(snapshotName, cancellationToken);

        IEnumerable<UnitModel> unitsList = GetUnitsList(unitsAtCountry);
        IEnumerable<UnitModel> unitsListSnapshot = GetUnitsList(unitsSnapshot?.Data);

        var difference = unitsList.ExceptBy(unitsListSnapshot.Select(u => u.Name), u => u.Name);

        foreach(var unit in difference)
        {
            var notification = new Notification
            {
                Payload = new NotificationPayload
                {
                    Text = $"There is new unit of {brand} - {unit.Name}! You can find it here👇",
                    Address = unit.Address?.Text,
                    Coordinates = unit.Coords,
                    Name = unit.Name
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }

        await UpdateSnapshot(snapshotName, unitsAtCountry, cancellationToken);
    }

    private async Task UpdateSnapshot<TData>(string snapshotName, TData data, CancellationToken cancellationToken)
    {
        var newSnapshot = Snapshot<TData>.Create(snapshotName, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
    }

    private IEnumerable<UnitModel> GetUnitsList(BrandData<UnitListModel> unitListModel)
    {
        return unitListModel?.Countries.SelectMany(c => c.Pizzerias).ToList() ?? new List<UnitModel>();
    }

    private async Task CheckAndNotify1000(KeyValuePair<string, int> totalAtCountry, Brands brand, CancellationToken cancellationToken)
    {
        if (!CheckRemainder1000(totalAtCountry.Value))
        {
            return;
        }

        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text =
                    $"There is {totalAtCountry.Value} units of {brand} at {totalAtCountry.Key}!"
            }
        };

        await _notificationsService.Save(notification, cancellationToken);
    }

    private async Task CheckDifferenceAndNotify(Brands brand, IEnumerable<string> difference, CancellationToken cancellationToken)
    {
        foreach(var countryName in difference)
        {
            var notification = new Notification
            {
                Payload = new NotificationPayload
                {
                    Text = $"There is new country of {brand} - {countryName}!"
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
    }

    private async Task UpdateUnitsOfBrandAtCountrySnapshot(Brands brand, int countryId, CancellationToken cancellationToken)
    {
        BrandData<UnitListModel> unitsAtCountry = await _globalApiClient.UnitsOfBrandAtCountry(brand, countryId, cancellationToken);

        var snapshotName = GetUnitsOfBrandAtCountrySnapshotName(brand, countryId);

        await UpdateSnapshot(snapshotName, unitsAtCountry, cancellationToken);
    }

    private string GetUnitsOfBrandAtCountrySnapshotName(Brands brand, int countryId)
    {
        return $"{nameof(_globalApiClient.UnitsOfBrandAtCountry)}{brand}{countryId}";
    }
}