using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Linq;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Application
{
    public abstract class CrudService<TEntity, TKey, TModel> :
        CrudService<TEntity, TKey, TModel, TModel>,
        ICrudService<TKey, TModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected CrudService(IUnitOfWork uow, IEventBus bus) : base(uow, bus)
        {
        }
    }

    public abstract class CrudService<TEntity, TKey, TReadModel, TModel> :
        CrudService<TEntity, TKey, TReadModel, TModel, FilteredPagedRequestModel>,
        ICrudService<TKey, TReadModel, TModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected CrudService(IUnitOfWork uow, IEventBus bus) : base(uow, bus)
        {
        }
    }

    public abstract class CrudService<TEntity, TKey, TReadModel, TModel,
        TFilteredPagedRequestModel> : InternalCrudService<TEntity, TKey, TReadModel,
        TModel, TFilteredPagedRequestModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedRequestModel : class, IFilteredPagedRequest
        where TKey : IEquatable<TKey>
    {
        protected readonly DbSet<TEntity> EntitySet;
        protected readonly IUnitOfWork UnitOfWork;

        protected CrudService(IUnitOfWork uow, IEventBus bus) : base(bus)
        {
            UnitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
            EntitySet = UnitOfWork.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> FindEntityQueryable => EntitySet.AsNoTracking();

        protected sealed override async Task<IReadOnlyList<TEntity>> FindEntityListAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await FindEntityQueryable.Where(predicate).ToListAsync(cancellationToken);
        }

        protected sealed override Task<IPagedResult<TEntity>> FindEntityPagedListAsync(PagedRequestModel model,
            CancellationToken cancellationToken = default)
        {
            return FindEntityQueryable.ToPagedListAsync(model, cancellationToken);
        }

        protected sealed override async Task CreateListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            EntitySet.AddRange(entityList);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            UnitOfWork.MarkUnchanged(entityList);
        }

        protected sealed override async Task UpdateListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            UnitOfWork.TrackChanges(entityList);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            UnitOfWork.MarkUnchanged(entityList);
        }

        protected sealed override Task RemoveListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            EntitySet.RemoveRange(entityList);
            return UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}