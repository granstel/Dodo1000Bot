using System.Collections.Generic;

namespace Dodo1000Bot.Models.RealtimeBoard;

public class Statistics
{
    public ICollection<RevenueStatistics> Revenues { get; set; }

    public IDictionary<string, int> RestaurantsCount { get; set; }

    public IDictionary<string, int> CountriesCount { get; set; }

    public int CountriesOverall { get; set; }

    public string EmployeesCount { get; set; }

    public int OrdersPerMinute { get; set; }
}