using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Services.Extensions;
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
    private readonly ICountriesService _countriesService;

    public UnitsService(
        ILogger<UnitsService> log, 
        IGlobalApiClient globalApiClient, 
        INotificationsService notificationsService, 
        ISnapshotsRepository snapshotsRepository, 
        ICountriesService countriesService)
    {
        _log = log;
        _globalApiClient = globalApiClient;
        _notificationsService = notificationsService;
        _snapshotsRepository = snapshotsRepository;
        _countriesService = countriesService;
    }

    public async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);
            _log.LogInformation("unitsCount: {unitsCount}", unitsCount.Serialize());
            await AboutTotalOverall(unitsCount, cancellationToken);
            await AboutTotalAtBrands(unitsCount, cancellationToken);
            await AboutTotalAtCountries(unitsCount, cancellationToken);
            await AboutTotalCountriesAtBrands(unitsCount, cancellationToken);

            var snapshotName = nameof(_globalApiClient.UnitsCount);
            var unitsCountSnapshot = 
                await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(snapshotName, cancellationToken);
            _log.LogInformation("unitsCountSnapshot: {unitsCountSnapshot}", unitsCountSnapshot.Serialize());
            await AboutNewCountries(unitsCount, unitsCountSnapshot.Data, cancellationToken);
            await AboutNewUnits(unitsCount, unitsCountSnapshot.Data, cancellationToken);
            _log.LogInformation("Before UpdateSnapshot at CheckAndNotify");
            await UpdateSnapshot(snapshotName, unitsCount, cancellationToken);
            _log.LogInformation("After UpdateSnapshot at CheckAndNotify");
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't check and notify units count");
        }
    }

    public async Task CreateUnitsCountSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        try
        {
            var snapshotName = nameof(_globalApiClient.UnitsCount);

            var unitsCountSnapshot =
                await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(snapshotName, cancellationToken);

            if (unitsCountSnapshot?.Data is not null)
            {
                _log.LogInformation("unitsCountSnapshot is not null");
                return;
            }

            var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);

            await UpdateSnapshot(snapshotName, unitsCount, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't {methodName}", nameof(CreateUnitsCountSnapshotIfNotExists));
        }
    }

    public async Task CreateUnitsSnapshotIfNotExists(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCountSnapshotName = nameof(_globalApiClient.UnitsCount);

            var unitsCountSnapshot =
                await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(unitsCountSnapshotName, cancellationToken);

            if (unitsCountSnapshot?.Data is null)
            {
                _log.LogInformation("unitsCountSnapshot is null");
                return;
            }
            
            List<Brands> brands = unitsCountSnapshot.Data.Brands.Select(b => b.Brand).ToList();

            foreach (var brand in brands)
            {
                var totalUnitCountListModel = unitsCountSnapshot.Data
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

    private async Task AboutTotalOverall(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalOverall = unitsCount.Brands.Sum(b => b.Total);

        if (!CheckHelper.CheckRemainder(totalOverall, 1000))
        {
            return;
        }

        var notification = new Notification(NotificationType.Emoji)
        {
            Payload = new NotificationPayload
            {
                Text = "😮",
                HappenedAt = DateTime.Now
            }
        };

        await _notificationsService.Save(notification, cancellationToken);

        notification = new Notification(NotificationType.TotalOverall)
        {
            Payload = new NotificationPayload
            {
                Text = $"Wow! 🎉 \r\nThere are {totalOverall} restaurants of all Dodo brands! 🥳 \r\n" +
                       $"You can see them all on https://realtime.dodobrands.io"
            }
        };

        await _notificationsService.Save(notification, cancellationToken);
    }

    private async Task AboutTotalAtBrands(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalAtBrands = unitsCount.Brands.ToDictionary(b => b.Brand, b => b.Total);

        foreach (var totalAtBrand in totalAtBrands)
        {
            if (!CheckHelper.CheckRemainder(totalAtBrand.Value, 1000))
            {
                continue;
            }

            var notification = new Notification(NotificationType.Emoji)
            {
                Payload = new NotificationPayload
                {
                    Text = "😮",
                    HappenedAt = DateTime.Now
                }
            };

            await _notificationsService.Save(notification, cancellationToken);

            notification = new Notification(NotificationType.TotalAtBrands)
            {
                Payload = new NotificationPayload
                {
                    Text = $"Wow! 🔥 \r\nThere are {totalAtBrand.Value} restaurants of {totalAtBrand.Key} brand! 🥳"
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
    }

    internal async Task AboutTotalAtCountries(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalAtBrandAtCountries = unitsCount.Brands
            .ToDictionary(b => b.Brand, b => b.Countries);

        foreach (var totalAtBrandAtCountry in totalAtBrandAtCountries)
        {
            var brand = totalAtBrandAtCountry.Key;
            foreach (var totalAtCountry in totalAtBrandAtCountry.Value)
            {
                await CheckAndNotify1000(totalAtCountry, brand, cancellationToken);
            }
        }
    }

    internal async Task AboutTotalCountriesAtBrands(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var countriesCountAtBrands = unitsCount.Brands
            .ToDictionary(b => b.Brand, b => b.Countries.Count());

        foreach (var countriesCountAtBrand in countriesCountAtBrands)
        {
            var brand = countriesCountAtBrand.Key;

            if (!CheckHelper.CheckRemainder(countriesCountAtBrand.Value, 10))
            {
                continue;
            }

            var notification = new Notification(NotificationType.TotalCountriesAtBrands)
            {
                Payload = new NotificationPayload
                {
                    Text = $"🌏 Awesome! {brand} is already in {countriesCountAtBrand.Value} countries!"
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
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

        Dictionary<Brands, List<UnitCountModel>> countriesAtBrand = GetCountriesAtBrands(unitsCount.Brands);
        Dictionary<Brands, List<UnitCountModel>> countriesAtBrandSnapshot = GetCountriesAtBrands(unitsCountSnapshot.Brands);

        foreach (var brand in countriesAtBrand.Keys)
        {
            List<UnitCountModel> countries = countriesAtBrand.GetValueOrDefault(brand);
            List<UnitCountModel> countriesSnapshot = GetValueOrDefault(countriesAtBrandSnapshot, brand, new List<UnitCountModel>());

            var difference = countries.ExceptBy(countriesSnapshot.Select(s => s.CountryName), c => c.CountryName);

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

    private Dictionary<Brands, List<UnitCountModel>> GetCountriesAtBrands(IEnumerable<BrandTotalUnitCountListModel> unitsCountBrands)
    {
        return unitsCountBrands.ToDictionary(b => b.Brand, b => b.Countries.ToList());
    }

    internal async Task AboutNewUnits(BrandListTotalUnitCountListModel unitsCount, BrandListTotalUnitCountListModel unitsCountSnapshot, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start AboutNewUnits");
        if (unitsCountSnapshot is null)
        {
            _log.LogInformation("unitsCountSnapshot is null");
            return;
        }

        List<Brands> brands = unitsCount.Brands.Select(b => b.Brand).ToList();

        foreach (var brand in brands)
        {
            var countriesOfBrand = unitsCount
                .Brands.First(b => b.Brand == brand).Countries;

            _log.LogInformation("Brand {brand}", brand);

            var restaurantsAtBrand = unitsCount.Brands.Where(b => b.Brand == brand).Select(b => b.Total).FirstOrDefault();
            var totalOverall = unitsCount.Brands.Sum(b => b.Total);

            foreach (var country in countriesOfBrand)
            {
                await CheckUnitsOfBrandAtCountryAndNotify(brand, country.CountryId, country.CountryCode, restaurantsAtBrand, totalOverall, cancellationToken);
            }
        }
        _log.LogInformation("Finish AboutNewUnits");
    }

    internal async Task CheckUnitsOfBrandAtCountryAndNotify(Brands brand, int countryId, string countryCode,
        int restaurantsCountAtBrand, int totalOverall, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start CheckUnitsOfBrandAtCountryAndNotify for brand {brand} at countryId {countryId}", brand, countryId);
        BrandData<UnitListModel> unitsAtCountry = await _globalApiClient.UnitsOfBrandAtCountry(brand, countryId, cancellationToken);

        _log.LogInformation("unitsAtCountry: {unitsAtCountry}", unitsAtCountry.Serialize());

        var snapshotName = GetUnitsOfBrandAtCountrySnapshotName(brand, countryId);
        var unitsSnapshot = 
            await _snapshotsRepository.Get<BrandData<UnitListModel>>(snapshotName, cancellationToken);

        _log.LogInformation("unitsSnapshot: {unitsSnapshot}", unitsSnapshot.Serialize());

        IEnumerable<UnitModel> unitsList = GetUnitsList(unitsAtCountry);
        IEnumerable<UnitModel> unitsListSnapshot = GetUnitsList(unitsSnapshot?.Data);

        _log.LogInformation("unitsList: {unitsList}", unitsList.Serialize());
        _log.LogInformation("unitsListSnapshot: {unitsListSnapshot}", unitsListSnapshot.Serialize());

        const string formatOfDestinctions = "{0}-{1}";

        var formattedDistinctions = unitsListSnapshot.Select(uls => string.Format(formatOfDestinctions, uls.Name, uls.StartDate));
        var difference = unitsList.ExceptBy(formattedDistinctions, 
                                            ul => string.Format(formatOfDestinctions, ul.Name, ul.StartDate))
            .Where(ul => ul.StartDate.Year == DateTime.Today.Date.Year).ToList();

        _log.LogInformation("difference: {difference}", difference.Serialize());

        var brandEmoji = Constants.BrandsEmoji.GetValueOrDefault(brand) ?? string.Empty;
        var flag = Constants.TelegramFlags.GetValueOrDefault(countryCode) ?? string.Empty;

        foreach (var unit in difference)
        {
            var notification = new Notification(NotificationType.NewUnit)
            {
                Payload = new NotificationPayload
                {
                    Text = $"Wow! There is new {brand}{brandEmoji} in {unit.Address?.Locality?.Name}{flag}! You can find it on the map👆 " +
                           $"\r\nIt's {restaurantsCountAtBrand} restaurant of {brand} and {totalOverall} of all Dodo brands 🔥",
                    Address = unit.Address?.Text,
                    Coordinates = unit.Coords,
                    Name = unit.Name
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
        _log.LogInformation("Before UpdateSnapshot at CheckUnitsOfBrandAtCountryAndNotify");
        await UpdateSnapshot(snapshotName, unitsAtCountry, cancellationToken);
        _log.LogInformation("After UpdateSnapshot at CheckUnitsOfBrandAtCountryAndNotify");
        _log.LogInformation("Finish CheckUnitsOfBrandAtCountryAndNotify for brand {brand} at countryId {countryId}", brand, countryId);
    }

    private async Task UpdateSnapshot<TData>(string snapshotName, TData data, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
        var newSnapshot = Snapshot<TData>.Create(snapshotName, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
        _log.LogInformation("Finish UpdateSnapshot for snapshotName {snapshotName}", snapshotName);
    }

    private IEnumerable<UnitModel> GetUnitsList(BrandData<UnitListModel> unitListModel)
    {
        return unitListModel?.Countries.SelectMany(c => c.Pizzerias).ToList() ?? new List<UnitModel>();
    }

    private async Task CheckAndNotify1000(UnitCountModel totalAtCountry, Brands brand, CancellationToken cancellationToken)
    {
        if (!CheckHelper.CheckRemainder(totalAtCountry.PizzeriaCount, 1000))
        {
            return;
        }

        var countryName = totalAtCountry.CountryName;

        try
        {
            countryName = await _countriesService.GetName(totalAtCountry.CountryCode, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't get country name with code {code} from {source}", 
                totalAtCountry.CountryCode, nameof(ICountriesService));
        }

        var notification = new Notification(NotificationType.Emoji)
        {
            Payload = new NotificationPayload
            {
                Text = "😮",
                HappenedAt = DateTime.Now
            }
        };

        await _notificationsService.Save(notification, cancellationToken);

        notification = new Notification(NotificationType.TotalAtCountries)
        {
            Payload = new NotificationPayload
            {
                Text =
                    $"Incredible! 🥳 \r\nThere are {totalAtCountry.PizzeriaCount} {brand} restaurants in the {countryName}! ❤️‍🔥"
            }
        };

        await _notificationsService.Save(notification, cancellationToken);
    }

    private async Task CheckDifferenceAndNotify(Brands brand, IEnumerable<UnitCountModel> difference, CancellationToken cancellationToken)
    {
        foreach(var countModel in difference)
        {
            var countryName = countModel.CountryName;

            try
            {
                countryName = await _countriesService.GetName(countModel.CountryCode, cancellationToken);
            }
            catch (Exception e)
            {
                _log.LogError(e, "Can't get country name with code {code} from {source}", 
                    countModel.CountryCode, nameof(ICountriesService));
            }

            var flag = Constants.TelegramFlags.GetValueOrDefault(countModel.CountryCode ?? string.Empty) ?? "🤩";
            var notification = new Notification(NotificationType.Emoji)
            {
                Payload = new NotificationPayload
                {
                    Text = flag,
                    HappenedAt = DateTime.Now
                }
            };

            await _notificationsService.Save(notification, cancellationToken);

            notification = new Notification(NotificationType.NewCountry)
            {
                Payload = new NotificationPayload
                {
                    Text = $"🌏 Wow! There is new country of {brand} - {countryName}! {flag}"
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
    }

    private string GetUnitsOfBrandAtCountrySnapshotName(Brands brand, int countryId)
    {
        return $"{nameof(_globalApiClient.UnitsOfBrandAtCountry)}{brand}{countryId}";
    }
}