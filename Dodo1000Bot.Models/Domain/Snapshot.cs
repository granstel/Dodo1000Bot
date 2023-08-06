using System;

namespace Dodo1000Bot.Models.Domain
{
    public class Snapshot<TData>
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public TData Data { get; init; }

        public DateTime ModifiedAt => DateTime.UtcNow;

        public static Snapshot<TData> Create(string name, TData data)
        {
            return new()
            {
                Name = name,
                Data = data,
            };
        }
    }
}
