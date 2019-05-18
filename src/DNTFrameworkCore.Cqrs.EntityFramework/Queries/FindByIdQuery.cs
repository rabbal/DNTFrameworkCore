using DNTFrameworkCore.Cqrs.Queries;
using DNTFrameworkCore.Domain.Entities;
using System;

namespace DNTFrameworkCore.Cqrs.EntityFramework.Queries
{
    public class FindByIdQuery<TKey, TEntity, TReadModel> : IQuery<TReadModel> where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>
    {
        public TKey Id { get; set; }
    }
}
