using System;
using DNTFrameworkCore.Cqrs.Queries;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Cqrs.EFCore.Queries
{
    public class FindByIdQuery<TKey, TEntity, TReadModel> : IQuery<TReadModel> where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>
    {
        public TKey Id { get; set; }
    }
}
