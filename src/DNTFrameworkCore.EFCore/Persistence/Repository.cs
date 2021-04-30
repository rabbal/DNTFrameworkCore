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

        public TEntity Find(TKey id)
        {
            return FindEntityQueryable.FirstOrDefault(IdEqualityExpression<TEntity, TKey>(id));
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> FindPagedListAsync(Expression<Func<TEntity, bool>> predicate,
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.Where(predicate).OrderBy(e => e.Id).Page(page, pageSize)
                .ToListAsync(cancellationToken);
        }

        public Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
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

        protected virtual IQueryable<TEntity> FindEntityQueryable => EntitySet;
    }
}