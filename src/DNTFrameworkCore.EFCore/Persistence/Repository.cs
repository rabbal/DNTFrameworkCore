using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Querying;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Persistence
{
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : AggregateRoot<TKey>
    {
        protected readonly DbSet<TEntity> EntitySet;
        protected readonly IDbContext DbContext;

        protected RepositoryBase(IDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            EntitySet = DbContext.Set<TEntity>();
        }

        public IReadOnlyList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return FindEntityQueryable.Where(predicate).ToList();
        }

        public IReadOnlyList<TEntity> FindPagedList(Expression<Func<TEntity, bool>> predicate, int page, int pageSize)
        {
            return FindEntityQueryable.Where(predicate).OrderBy(e => e.Id).Page(page, pageSize).ToList();
        }

        public Maybe<TEntity> Find(TKey id)
        {
            return FindEntityQueryable.FirstOrDefault(IdEqualityExpression(id));
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> FindPagedListAsync(Expression<Func<TEntity, bool>> predicate,
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.Where(predicate).OrderBy(e => e.Id).Page(page, pageSize).ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<Maybe<TEntity>> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.FirstOrDefaultAsync(IdEqualityExpression(id), cancellationToken: cancellationToken);
        }

        public virtual void Add(TEntity entity)
        {
            EntitySet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entityList)
        {
            EntitySet.AddRange(entityList);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await EntitySet.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default)
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

        public long Count()
        {
            return EntitySet.LongCount();
        }

        public long Count(Expression<Func<TEntity, bool>> predicate)
        {
            return EntitySet.LongCount(predicate);
        }

        public Task<long> CountAsync(CancellationToken cancellationToken = default)
        {
            return EntitySet.LongCountAsync(cancellationToken);
        }

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.LongCountAsync(predicate, cancellationToken);
        }

        protected virtual IQueryable<TEntity> FindEntityQueryable => EntitySet;

        private static Expression<Func<TEntity, bool>> IdEqualityExpression(TKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, nameof(Entity<TKey>.Id)),
                Expression.Constant(id, typeof(TKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}