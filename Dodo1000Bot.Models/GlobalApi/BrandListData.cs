using System.Collections.Generic;

namespace Dodo1000Bot.Models.GlobalApi
{
    public class BrandListData<T>
    {
        public IEnumerable<BrandData<T>> Brands { get; set; }
    }
}