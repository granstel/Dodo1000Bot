namespace Dodo1000Bot.Models.Domain
{
    public class Snapshot<TData>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TData Data { get; set; }
    }
}
