using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Domain
{
    public interface IMultiLingualEntity<TTranslation, TKey>
        where TTranslation : Entity<TKey>, IEntityTranslation where TKey : IEquatable<TKey>
    {
        ICollection<TTranslation> Translations { get; set; }
    }
}