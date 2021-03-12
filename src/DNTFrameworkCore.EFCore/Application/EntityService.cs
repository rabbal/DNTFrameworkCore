using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Application
{
    public abstract class EntityService<TEntity, TModel> :
        EntityService<TEntity, int, TModel, TModel>,
        IEntityService<int, TModel>
        where TEntity : Entity<int>, new()
        where TModel : MasterModel<int>
    {
        protected EntityService(IDbContext dbContext, IEventBus bus) : base(dbContext, bus)
        {
        }
    }
    
    public abstract class EntityService<TEntity, TKey, TModel> :
        EntityService<TEntity, TKey, TModel, TModel>,
        IEntityService<TKey, TModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected EntityService(IDbContext dbContext, IEventBus bus) : base(dbContext, bus)
        {
        }
    }

    public abstract class EntityService<TEntity, TKey, TReadModel, TModel> :
        EntityService<TEntity, TKey, TReadModel, TModel, FilteredPagedRequest>,
        IEntityService<TKey, TReadModel, TModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected EntityService(IDbContext dbContext, IEventBus bus) : base(dbContext, bus)
        {
        }
    }

    public abstract class EntityService<TEntity, TKey, TReadModel, TModel,
        TFilteredPagedRequest> : EntityServiceBase<TEntity, TKey, TReadModel,
        TModel, TFilteredPagedRequest>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedRequest : IFilteredPagedRequest
        where TKey : IEquatable<TKey>
    {
        protected readonly DbSet<TEntity> EntitySet;
        protected readonly IDbContext DbContext;

        protected EntityService(IDbContext dbContext, IEventBus bus) : base(bus)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            EntitySet = DbContext.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> FindEntityQueryable => EntitySet.AsNoTracking();

        protected sealed override async Task<IReadOnlyList<TEntity>> FindEntityListAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await FindEntityQueryable.Where(predicate).ToListAsync(cancellationToken);
        }

        protected sealed override Task<IPagedResult<TEntity>> FindEntityPagedListAsync(IPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return FindEntityQueryable.ToPagedListAsync(request, cancellationToken);
        }

        protected sealed override async Task CreateEntityListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            await EntitySet.AddRangeAsync(entityList, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            DbContext.MarkUnchanged(entityList);
        }

        protected sealed override async Task UpdateEntityListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            DbContext.UpdateGraph(entityList);
            await DbContext.SaveChangesAsync(cancellationToken);
            DbContext.MarkUnchanged(entityList);
        }

        protected sealed override Task RemoveEntityListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            EntitySet.RemoveRange(entityList);
            return DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}