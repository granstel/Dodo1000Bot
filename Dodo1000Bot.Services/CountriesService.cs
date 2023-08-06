using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Services;

public class CountriesService : ICountriesService
{
    private readonly ILogger<CountriesService> _log;
    private readonly ICountriesRepository _repository;
    private readonly IRestcountriesApiClient _client;

    public CountriesService(ILogger<CountriesService> log, ICountriesRepository repository, IRestcountriesApiClient client)
    {
        _log = log;
        _repository = repository;
        _client = client;
    }

    public async Task<string> GetName(int id, string code, CancellationToken cancellationToken)
    {
        var name = await _repository.GetName(id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        try
        {
            var countries = await _client.GetNames(cancellationToken, code);

            name = countries.Select(c => c.Name.Common).FirstOrDefault();
        }
        catch (Exception e)
        {
            _log.LogError(e, "Can't get name for country with code {code}", code);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            await _repository.Save(code, name, cancellationToken);
        }

        return name;
    }
}