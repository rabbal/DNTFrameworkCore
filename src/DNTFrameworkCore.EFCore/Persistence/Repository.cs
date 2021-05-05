using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Querying;
using Microsoft.EntityFrameworkCore;
using static DNTFrameworkCore.Extensions.EntityExtensions;

namespace DNTFrameworkCore.EFCore.Persistence
{
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>, IAggregateRoot
    {
        protected readonly DbSet<TEntity> EntitySet;
        protected readonly IDbContext DbContext;

        protected RepositoryBase(IDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            EntitySet = DbContext.Set<TEntity>();
        }

        public virtual IReadOnlyList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return FindEntityQueryable.Where(predicate).ToList();
        }

        public virtual IReadOnlyList<TEntity> FindPagedList(Expression<Func<TEntity, bool>> predicate, int page, int pageSize)
        {
            return FindEntityQueryable.Where(predicate).OrderBy(e => e.Id).Page(page, pageSize).ToList();
        }

        public virtual TEntity Find(TKey id)
        {
            return FindEntityQueryable.FirstOrDefault(IdEqualityExpression<TEntity, TKey>(id));
        }

        public virtual async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<TEntity>> FindPagedListAsync(Expression<Func<TEntity, bool>> predicate,
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.Where(predicate).OrderBy(e => e.Id).Page(page, pageSize)
                .ToListAsync(cancellationToken);
        }

        public virtual Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return FindEntityQueryable.FirstOrDefaultAsync(IdEqualityExpression<TEntity, TKey>(id), cancellationToken);
        }

        public virtual void Add(TEntity entity)
        {
            EntitySet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entityList)
        {
            EntitySet.AddRange(entityList);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await EntitySet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default)
        {
            await EntitySet.AddRangeAsync(entityList, cancellationToken);
        }

        public virtual void Remove(TEntity entity)
        {
            EntitySet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entityList)
        {
            EntitySet.RemoveRange(entityList);
        }

        protected virtual IQueryable<TEntity> FindEntityQueryable => EntitySet;
    }
}