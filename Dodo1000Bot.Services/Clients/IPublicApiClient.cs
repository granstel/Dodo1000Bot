using System.Threading;
using System.Threading.Tasks;
using Dodo1000Bot.Models.PublicApi;

namespace Dodo1000Bot.Services.Clients;

public interface IPublicApiClient
{
    Task<UnitInfo[]> UnitInfo(string brand, string countryCode, CancellationToken cancellationToken);

    Task<Department> GetDepartmentById(string brand, string countryCode, int departmentId,
        CancellationToken cancellationToken);
}