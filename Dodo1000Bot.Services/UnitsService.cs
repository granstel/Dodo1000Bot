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
using Dodo1000Bot.Services.Extensions;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Dodo1000Bot.Services.Tests")]

namespace Dodo1000Bot.Services;
using AllUnitsDictionary = Dictionary<Brands, Dictionary<UnitCountModel, IEnumerable<UnitInfo>>>;
public class UnitsService : CheckAndNotifyService
{
    private readonly ILogger<UnitsService> _log;
    private readonly INotificationsService _notificationsService;
    private readonly IGlobalApiService _globalApiService;
    private readonly IPublicApiService _publicApiService;
    private readonly ICountriesService _countriesService;

    public UnitsService(
        ILogger<UnitsService> log, 
        INotificationsService notificationsService, 
        IGlobalApiService globalApiService, 
        IPublicApiService publicApiService, 
        ICountriesService countriesService)
    {
        _log = log;
        _notificationsService = notificationsService;
        _globalApiService = globalApiService;
        _publicApiService = publicApiService;
        _countriesService = countriesService;
    }

    public override async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        try
        {
            var unitsCount = await _globalApiService.GetUnitsCount(cancellationToken);
            _log.LogInformation("unitsCount: {unitsCount}", unitsCount.Serialize());
            await AboutTotalOverall(unitsCount, cancellationToken);
            await AboutTotalAtBrands(unitsCount, cancellationToken);
            await AboutTotalAtCountries(unitsCount, cancellationToken);
            await AboutTotalCountriesAtBrands(unitsCount, cancellationToken);

            var unitsCountSnapshot = await _globalApiService.GetUnitsCountSnapshot(cancellationToken);
            _log.LogInformation("unitsCountSnapshot: {unitsCountSnapshot}", unitsCountSnapshot.Serialize());
            await AboutNewCountries(unitsCount, unitsCountSnapshot, cancellationToken);

            var allUnits = await _publicApiService.GetAllUnits(unitsCountSnapshot.Brands.ToList(), cancellationToken);

            await AboutNewUnits(allUnits, cancellationToken);

            await _globalApiService.UpdateUnitsCountSnapshot(cancellationToken);
            await _publicApiService.UpdateAllUnitsSnapshot(allUnits, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't check and notify units count");
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

    internal async Task AboutNewUnits(AllUnitsDictionary allUnits, CancellationToken cancellationToken)
    {
        _log.LogInformation("Start AboutNewUnits");
        // TODO: get brands from global API endpoint
        var brands = allUnits.Keys;

        var totalOverall = allUnits.Sum(b => b.Value.Sum(c => c.Value.Count()));

        foreach (var brand in brands)
        {
            Dictionary<UnitCountModel, IEnumerable<UnitInfo>> allUnitsAtBrand = allUnits.GetValueOrDefault(brand);
            // TODO: get countries from global API endpoint
            var countriesOfBrand = allUnitsAtBrand.Keys;

            _log.LogInformation("Brand {brand}", brand);

            var restaurantsAtBrand = allUnitsAtBrand.Values.Sum(c => c.Count());

            foreach (var country in countriesOfBrand)
            {
                var unitsList = allUnitsAtBrand.GetValueOrDefault(country);
                var unitsListSnapshot = await _publicApiService.GetUnitInfoOfBrandAtCountrySnapshot(brand, country.CountryCode, cancellationToken);

                await CheckUnitsOfBrandAtCountryAndNotify(unitsList, unitsListSnapshot, brand, country.CountryCode, restaurantsAtBrand, totalOverall, cancellationToken);
            }
        }
        _log.LogInformation("Finish AboutNewUnits");
    }

    internal async Task CheckUnitsOfBrandAtCountryAndNotify(
        IEnumerable<UnitInfo> unitsList, 
        IEnumerable<UnitInfo> unitsListSnapshot, 
        Brands brand,
        string countryCode,
        int restaurantsCountAtBrand, 
        int totalOverall, 
        CancellationToken cancellationToken)
    {
        _log.LogInformation("Start CheckUnitsOfBrandAtCountryAndNotify for brand {brand} at countryCode {countryCode}", brand, countryCode);

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
                    Name = unit.Name
                }
            };

            if (unit.Location is { Latitude: not null, Longitude: not null })
            {
                notification.Payload.Coordinates = new Coordinates
                {
                    Latitude = unit.Location.Latitude.Value,
                    Longitude = unit.Location.Longitude.Value
                };
            }
            
            await _notificationsService.Save(notification, cancellationToken);
        }
        _log.LogInformation("Finish CheckUnitsOfBrandAtCountryAndNotify for brand {brand} at countryId {countryCode}", brand, countryCode);
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
}