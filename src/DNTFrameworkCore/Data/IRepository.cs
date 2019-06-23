using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Data
{
    /// <summary>
    /// Implement per AggregateRoot and use it as internal Repository inside ApplicationLayer when your domain is Rich Domain, otherwise you don't need it and CrudService is fine
    /// </summary>
    public interface IRepository<TEntity, in TKey> where TEntity : AggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<Maybe<TEntity>> FindAsync(Predicate<TEntity> predicate);
        Task<IReadOnlyList<TEntity>> FindListAsync(Predicate<TEntity> predicate);
        Task UpdateAsync(TEntity entity);
        Task UpdateAsync(IEnumerable<TEntity> entityList);
        Task InsertAsync(TEntity entity);
        Task InsertAsync(IEnumerable<TEntity> entityList);
        Task SaveAsync(TEntity entity);
        Task SaveAsync(IEnumerable<TEntity> entityList);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(IEnumerable<TEntity> entityList);
        Task DeleteAsync(TKey id);
        Task DeleteAsync(IEnumerable<TKey> ids);
    }
}