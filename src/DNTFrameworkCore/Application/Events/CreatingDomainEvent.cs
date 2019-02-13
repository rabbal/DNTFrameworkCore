using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Eventing;

namespace DNTFrameworkCore.Application.Events
{
    public class CreatingDomainEvent<TModel, TKey> : IDomainEvent
        where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
    {
        public CreatingDomainEvent(IEnumerable<TModel> models)
        {
            Models = models.ToImmutableList();
        }

        public IReadOnlyList<TModel> Models { get; }
    }
}