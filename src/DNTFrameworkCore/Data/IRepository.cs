using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Data
{
    /// <summary>
    /// Implement per AggregateRoot and use it as internal Repository inside ApplicationLayer when your domain is Rich Domain, otherwise you don't need it and EntityService is fine
    /// Under development for DDD purpose just as internal repositories
    /// </summary>
    public interface IRepository<TEntity, in TKey> where TEntity : AggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<Maybe<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<Maybe<TEntity>> FindAsync(TKey id);
        Task<IReadOnlyList<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TEntity>> FindListAsync(IEnumerable<TKey> ids);
        Task<IPagedResult<TEntity>> FindPagedListAsync(Expression<Func<TEntity, bool>> predicate);
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
        Task DeleteAsync(Expression<Predicate<TEntity>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<long> LongCountAsync();
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}