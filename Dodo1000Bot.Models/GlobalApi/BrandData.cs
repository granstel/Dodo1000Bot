namespace Dodo1000Bot.Models.GlobalApi
{
    public class BrandData<T> : ResultListModel<T>
    {
        public Brands Brand { get; set; }
    }
}