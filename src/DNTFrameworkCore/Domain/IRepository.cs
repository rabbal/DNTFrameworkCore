using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Domain
{
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IAggregateRoot
    {
    }

    public interface IRepository<TEntity, in TKey> : IScopedDependency where TEntity : class, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        IReadOnlyList<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        IReadOnlyList<TEntity> FindPagedList(Expression<Func<TEntity, bool>> predicate, int page, int pageSize);
        TEntity Find(TKey id);

        Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> FindPagedListAsync(Expression<Func<TEntity, bool>> predicate, int page,
            int pageSize, CancellationToken cancellationToken = default);

        Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entityList);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entityList);
    }
}