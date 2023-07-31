using System;

namespace Dodo1000Bot.Models.Domain
{
    public class Snapshot<TData>
    {
        public Snapshot(TData data)
        {
            Data = data;
        }

        public int Id { get; init; }

        public string Name { get; init; }

        public TData Data { get; init; }

        public DateTime ModifiedAt => DateTime.UtcNow;
    }
}
