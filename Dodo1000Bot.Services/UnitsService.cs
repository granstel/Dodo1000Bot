using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.GlobalApi;

namespace Dodo1000Bot.Services;

public class UnitsService : IUnitsService
{
    private IGlobalApiClient _globalApiClient;
    private INotifyService _notifyService;

    public UnitsService(IGlobalApiClient globalApiClient, INotifyService notifyService)
    {
        _globalApiClient = globalApiClient;
        _notifyService = notifyService;
    }

    public async Task CheckAndNotify(CancellationToken cancellationToken)
    {
        var unitsCount = await _globalApiClient.UnitsCount(cancellationToken);

        await AboutTotalAtBrands(unitsCount, cancellationToken);
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

            var notification = $"There is {totalAtBrand.Value} units of {totalAtBrand.Key} brand";
            await _notifyService.Notify(notification, cancellationToken);
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

                var notification = $"There is {totalAtCountry.Value} units of {totalAtBrandAtCountry.Key} at {totalAtCountry.Key}";
                await _notifyService.Notify(notification, cancellationToken);
            }
        }
    }

    private bool CheckTheRule(int value)
    {
        return value % 1000 != 0;
    }
}