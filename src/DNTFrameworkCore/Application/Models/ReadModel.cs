using System;

namespace DNTFrameworkCore.Application.Models
{
    public abstract class ReadModel : ReadModel<int>
    {
    }

    public abstract class ReadModel<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
}
