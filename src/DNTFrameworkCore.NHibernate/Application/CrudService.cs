using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.NHibernate.Linq;
using DNTFrameworkCore.Querying;
using NHibernate;
using NHibernate.Linq;

namespace DNTFrameworkCore.NHibernate.Application
{
    public abstract class CrudService<TEntity, TKey, TModel> :
        CrudService<TEntity, TKey, TModel, TModel>,
        ICrudService<TKey, TModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected CrudService(ISession session, IEventBus bus) : base(session, bus)
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
        protected CrudService(ISession session, IEventBus bus) : base(session, bus)
        {
        }
    }

    public abstract class CrudService<TEntity, TKey, TReadModel, TModel,
        TFilteredPagedRequestModel> : DNTFrameworkCore.Application.CrudService<TEntity, TKey, TReadModel,
        TModel, TFilteredPagedRequestModel>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedRequestModel : class, IFilteredPagedRequest
        where TKey : IEquatable<TKey>
    {
        protected ISession Session { get; }
        protected IQueryable<TEntity> EntitySet { get; }

        protected CrudService(ISession session, IEventBus bus) : base(bus)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));

            EntitySet = session.Query<TEntity>();
        }

        protected virtual IQueryable<TEntity> FindEntityQueryable => Session.Query<TEntity>();
        
        protected sealed override async Task<IReadOnlyList<TEntity>> FindEntityListAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await FindEntityQueryable.Where(predicate).ToListAsync(cancellationToken);
        }

        protected sealed override async Task<IPagedResult<TEntity>> FindEntityPagedListAsync(PagedRequestModel model,
            CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.ToPagedListAsync(model, cancellationToken);
        }

        protected sealed override Task CreateListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            return Session.SaveAsync(entityList, cancellationToken);
        }

        protected sealed override Task UpdateListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            return Session.UpdateAsync(entityList, cancellationToken);
        }

        protected sealed override Task RemoveListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            return Session.DeleteAsync(entityList, cancellationToken);
        }
    }
}