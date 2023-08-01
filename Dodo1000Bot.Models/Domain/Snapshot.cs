using System;

namespace Dodo1000Bot.Models.Domain
{
    public class Snapshot<TData>
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public TData Data { get; init; }

        public DateTime ModifiedAt => DateTime.UtcNow;

        public static Snapshot<TData> Create(Snapshot<TData> oldSnapshot, TData data)
        {
            return new()
            {
                Id = oldSnapshot.Id,
                Name = oldSnapshot.Name,
                Data = data,
            };
        }
    }
}
