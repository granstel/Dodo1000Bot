namespace Dodo1000Bot.Models.GlobalApi
{
    public class TotalUnitCountListModel : ResultListModel<UnitCountModel>
    {
        public int Total { get; set; }
    }
}