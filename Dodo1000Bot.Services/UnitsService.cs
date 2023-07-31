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

            var snapshotName = nameof(_globalApiClient.UnitsCount);
            var unitsCountSnapshot = 
                await _snapshotsRepository.Get<BrandListTotalUnitCountListModel>(snapshotName, cancellationToken);

            await AboutTotalOverall(unitsCount, cancellationToken);
            await AboutTotalAtBrands(unitsCount, cancellationToken);
            await AboutTotalAtCountries(unitsCount, cancellationToken);

            await UpdateSnapshot(unitsCountSnapshot, unitsCount, cancellationToken);
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't check and notify units count");
        }
    }

    private async Task UpdateSnapshot<TData>(Snapshot<TData> oldSnapshot, TData data, CancellationToken cancellationToken)
    {
        var newSnapshot = Snapshot<TData>.Create(oldSnapshot, data);

        await _snapshotsRepository.Save(newSnapshot, cancellationToken);
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
        var totalAtBrandAtCountries = unitsCount.Brands.ToDictionary(b => b.Brand, b => b.Countries.ToDictionary(c => c.CountryName, c => c.PizzeriaCount));

        foreach (var totalAtBrandAtCountry in totalAtBrandAtCountries)
        {
            var brand = totalAtBrandAtCountry.Key;
            foreach (var totalAtCountry in totalAtBrandAtCountry.Value)
            {
                await CheckAndNotify0(totalAtCountry, brand, cancellationToken);
                await CheckAndNotify1000(totalAtCountry, brand, cancellationToken);
            }
        }
    }

    private async Task CheckAndNotify0(KeyValuePair<string, int> totalAtCountry, Brands brand, CancellationToken cancellationToken)
    {
        if (!CheckEquals0(totalAtCountry.Value))
        {
            return;
        }

        var notification = new Notification
        {
            Payload = new NotificationPayload
            {
                Text =
                    $"There is new country of {brand} - {totalAtCountry.Key}!"
            }
        };

        await _notificationsService.Save(notification, cancellationToken);
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
}