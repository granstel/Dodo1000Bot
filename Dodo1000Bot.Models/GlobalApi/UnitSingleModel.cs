namespace Dodo1000Bot.Models.GlobalApi
{
    public class UnitSingleModel
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public UnitModel Pizzeria { get; set; }
    }
}