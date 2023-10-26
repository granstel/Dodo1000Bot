using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models;
using Dodo1000Bot.Models.PublicApi;

namespace Dodo1000Bot.Services.Clients;

public interface IPublicApiClient
{
    Task<UnitInfo[]> UnitInfo(Brands brand, string countryCode, CancellationToken cancellationToken);

    Task<Department> GetDepartmentById(Brands brand, string countryCode, int departmentId, CancellationToken cancellationToken);
}