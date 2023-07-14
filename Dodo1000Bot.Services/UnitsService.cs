using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.Domain;
using Dodo1000Bot.Models.GlobalApi;

namespace Dodo1000Bot.Services;

public class UnitsService : IUnitsService
{
    private readonly IGlobalApiClient _globalApiClient;
    private readonly INotificationsService _notificationsService;

    public UnitsService(IGlobalApiClient globalApiClient, INotificationsService notificationsService)
    {
        _globalApiClient = globalApiClient;
        _notificationsService = notificationsService;
    }

    public async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);

        await AboutTotalOverall(unitsCount, cancellationToken);
        await AboutTotalAtBrands(unitsCount, cancellationToken);
        await AboutTotalAtCountries(unitsCount, cancellationToken);
    }

    private async Task AboutTotalOverall(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalOverall = unitsCount.Brands.Sum(b => b.Total);

        if (!CheckTheRule(totalOverall))
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
            if (!CheckTheRule(totalAtBrand.Value))
            {
                continue;
            }

            var notification = new Notification
            {
                Payload = new NotificationPayload
                {
                    Text = $"There is {totalAtBrand.Value} units of {totalAtBrand.Key} brand"
                }
            };
            await _notificationsService.Save(notification, cancellationToken);
        }
    }

    private async Task AboutTotalAtCountries(BrandListTotalUnitCountListModel unitsCount, CancellationToken cancellationToken)
    {
        var totalAtBrandAtCountries = unitsCount.Brands.ToDictionary(b => b.Brand, b => b.Countries.ToDictionary(c => c.CountryName, c => c.PizzeriaCount));

        foreach (var totalAtBrandAtCountry in totalAtBrandAtCountries)
        {
            foreach (var totalAtCountry in totalAtBrandAtCountry.Value)
            {
                if (!CheckTheRule(totalAtCountry.Value))
                {
                    continue;
                }

                var notification = new Notification
                {
                    Payload = new NotificationPayload
                    {
                        Text =
                            $"There is {totalAtCountry.Value} units of {totalAtBrandAtCountry.Key} at {totalAtCountry.Key}"
                    }
                };
                await _notificationsService.Save(notification, cancellationToken);
            }
        }
    }

    private bool CheckTheRule(int value)
    {
        return value % 1000 == 0;
    }
}