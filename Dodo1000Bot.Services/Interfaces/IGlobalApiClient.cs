using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.GlobalApi;

namespace Dodo1000Bot.Services
{
    public interface IGlobalApiClient
    {
        Task<BrandListTotalUnitCountListModel> UnitsCount(CancellationToken cancellationToken);

        Task<BrandData<UnitListModel>> UnitsOfBrandAtCountry(Brands brand, int countryId,
            CancellationToken cancellationToken);

    }
}