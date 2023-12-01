using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;
using Dodo1000Bot.Models.PublicApi;
using Dodo1000Bot.Services.Clients;
using Dodo1000Bot.Services.Extensions;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Dodo1000Bot.Services.Tests")]

namespace Dodo1000Bot.Services;
using AllUnitsDictionary = Dictionary<Brands, Dictionary<UnitCountModel, IEnumerable<UnitInfo>>>;
public class UnitsService : CheckAndNotifyService
{
    private readonly ILogger<UnitsService> _log;
    private readonly IGlobalApiClient _globalApiClient;
    private readonly IPublicApiClient _publicApiClient;
    private readonly INotificationsService _notificationsService;
    private readonly ISnapshotsRepository _snapshotsRepository;
    private readonly ICountriesService _countriesService;

    public UnitsService(
        ILogger<UnitsService> log, 
        IGlobalApiClient globalApiClient, 
        IPublicApiClient publicApiClient, 
        INotificationsService notificationsService, 
        ISnapshotsRepository snapshotsRepository, 
        ICountriesService countriesService)
    {
        _log = log;
        _globalApiClient = globalApiClient;
        _publicApiClient = publicApiClient;
        _notificationsService = notificationsService;
        _snapshotsRepository = snapshotsRepository;
        _countriesService = countriesService;
    }

    public override async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);
            _log.LogInformation("unitsCount: {unitsCount}", unitsCount.Serialize());
            await AboutTotalOverall(unitsCount, cancellationToken);
            await AboutTotalAtBrands(unitsCount, cancellationToken);
            await AboutTotalAtCountries(unitsCount, cancellationToken);
            await AboutTotalCountriesAtBrands(unitsCount, cancellationToken);

            var unitsCountSnapshot = await GetUnitsCountSnapshot(cancellationToken);
            _log.LogInformation("unitsCountSnapshot: {unitsCountSnapshot}", unitsCountSnapshot.Serialize());
            await AboutNewCountries(unitsCount, unitsCountSnapshot.Data, cancellationToken);

            var allUnits = await GetAbsolutelyAllUnits(unitsCount, cancellationToken);

            await AboutNewUnits(allUnits, cancellationToken);

            await UpdateUnitsCountSnapshot(unitsCount, cancellationToken);
            await UpdateAllUnitsSnapshot(allUnits, cancellationToken);
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

    private async Task<AllUnitsDictionary> GetAbsolutelyAllUnits(BrandListTotalUnitCountListModel globalApiUnitsCount, CancellationToken cancellationToken)
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

    internal async Task AboutNewUnits(AllUnitsDictionary allUnits, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start AboutNewUnits");

        var brands = allUnits.Keys;

        foreach (var brand in brands)
        {
            Dictionary<UnitCountModel, IEnumerable<UnitInfo>> allUnitsAtBrand = allUnits.GetValueOrDefault(brand);
            var countriesOfBrand = allUnitsAtBrand.Keys;

            _log.LogInformation("Brand {brand}", brand);

            var restaurantsAtBrand = allUnitsAtBrand.Values.Sum(c => c.Count());
            var totalOverall = allUnits.Sum(b => b.Value.Sum(c => c.Value.Count()));

            foreach (var country in countriesOfBrand)
            {
                var unitsList = allUnitsAtBrand.GetValueOrDefault(country);

                await CheckUnitsOfBrandAtCountryAndNotify(unitsList, brand, country.CountryId, country.CountryCode, restaurantsAtBrand, totalOverall, cancellationToken);
            }
        }
        _log.LogInformation("Finish AboutNewUnits");
    }

    internal async Task CheckUnitsOfBrandAtCountryAndNotify(IEnumerable<UnitInfo> unitsList, Brands brand, int countryId, string countryCode,
        int restaurantsCountAtBrand, int totalOverall, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start CheckUnitsOfBrandAtCountryAndNotify for brand {brand} at countryId {countryId}", brand, countryId);

        var unitsSnapshot = await GetUnitInfoOfBrandAtCountrySnapshot(brand, countryId, cancellationToken);

        var unitsListSnapshot = unitsSnapshot?.Data;

        _log.LogInformation("unitsList: {unitsList}", unitsList.Serialize());
        _log.LogInformation("unitsListSnapshot: {unitsListSnapshot}", unitsListSnapshot.Serialize());

        const string formatOfDistinctions = "{0}-{1}";

        var formattedDistinctions = unitsListSnapshot.Select(uls => string.Format(formatOfDistinctions, uls.Name, uls.BeginDateWork));
        var difference = unitsList.ExceptBy(formattedDistinctions, 
                                            ul => string.Format(formatOfDistinctions, ul.Name, ul.BeginDateWork))
            .Where(ul => ul.BeginDateWork.Year == DateTime.Today.Date.Year).ToList();

        _log.LogInformation("difference: {difference}", difference.Serialize());

        var brandEmoji = Constants.BrandsEmoji.GetValueOrDefault(brand) ?? string.Empty;
        var flag = Constants.TelegramFlags.GetValueOrDefault(countryCode) ?? string.Empty;

        foreach (var unit in difference)
        {
            var notification = new Notification(NotificationType.NewUnit)
            {
                Payload = new NotificationPayload
                {
                    Text = $"Wow! There is new {brand}{brandEmoji} in {unit.AddressDetails?.LocalityName}{flag}! You can find it on the map👆 " +
                           $"\r\nIt's {restaurantsCountAtBrand} restaurant of {brand} and {totalOverall} of all Dodo brands 🔥",
                    Address = unit.Address,
                    Coordinates = new()
                    {
                        Lat = unit.Location.Latitude,
                        Long = unit.Location.Longitude
                    },
                    Name = unit.Name
                }
            };

            await _notificationsService.Save(notification, cancellationToken);
        }
        _log.LogInformation("Finish CheckUnitsOfBrandAtCountryAndNotify for brand {brand} at countryId {countryId}", brand, countryId);
    }

    private async Task<Snapshot<BrandListTotalUnitCountListModel>> GetUnitsCountSnapshot(CancellationToken cancellationToken)
    {
        var snapshotName = nameof(_globalApiClient.UnitsCount);
        var unitsCountSnapshot =
            await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(snapshotName, cancellationToken);

        return unitsCountSnapshot;
    }

    private async Task<Snapshot<IEnumerable<UnitInfo>>> GetUnitInfoOfBrandAtCountrySnapshot(Brands brand, int countryId, CancellationToken cancellationToken)
    {
        var snapshotName = GetUnitInfoOfBrandAtCountrySnapshotName(brand, countryId);
        var unitsSnapshot =
            await _snapshotsRepository.Get<IEnumerable<UnitInfo>>(snapshotName, cancellationToken);

        return unitsSnapshot;
    }

    private async Task UpdateUnitsCountSnapshot(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var snapshotName = nameof(_globalApiClient.UnitsCount);
        await UpdateSnapshot(snapshotName, unitsCount, cancellationToken);
    }

    private async Task UpdateAllUnitsSnapshot(AllUnitsDictionary allUnits, CancellationToken cancellationToken)
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

    private string GetUnitInfoOfBrandAtCountrySnapshotName(Brands brand, int countryId)
    {
        return $"{nameof(_publicApiClient.UnitInfo)}{brand}{countryId}";
    }
}