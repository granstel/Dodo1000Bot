namespace Dodo1000Bot.Models.RealtimeBoard;

public class RevenueStatistics
{
    public RevenueTypes Type { get; set; }

    public decimal Revenue { get; set; }

    public string Currency { get; set; }

    public int? IncreasePercentage { get; set; }
}