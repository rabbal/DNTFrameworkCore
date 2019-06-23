using System;

namespace DNTFrameworkCore.Domain
{
    public interface IEntityTranslation
    {
        string Language { get; set; }
    }

    public interface IEntityTranslation<TEntity, TKey> : IEntityTranslation
        where TKey : IEquatable<TKey>
    {
        TEntity Entity { get; set; }
        TKey EntityId { get; set; }
    }
}