using System.Collections.Generic;

namespace Dodo1000Bot.Models.GlobalApi
{
    public class ResultListModel<T>
    {
        public IEnumerable<T> Countries { get; set; }

        public IEnumerable<ErrorModel> Errors { get; set; }
    }
}