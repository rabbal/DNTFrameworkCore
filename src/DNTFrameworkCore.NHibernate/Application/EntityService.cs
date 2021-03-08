using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.NHibernate.Linq;
using DNTFrameworkCore.Querying;
using NHibernate;
using NHibernate.Linq;

namespace DNTFrameworkCore.NHibernate.Application
{
    public abstract class EntityService<TEntity, TModel> :
        EntityService<TEntity, int, TModel, TModel>,
        IEntityService<int, TModel>
        where TEntity : Entity<int>, new()
        where TModel : MasterModel<int>
    {
        protected EntityService(ISession session, IEventBus bus) : base(session, bus)
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
        protected EntityService(ISession session, IEventBus bus) : base(session, bus)
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
        protected EntityService(ISession session, IEventBus bus) : base(session, bus)
        {
        }
    }

    public abstract class EntityService<TEntity, TKey, TReadModel, TModel,
        TFilteredPagedRequest> : EntityServiceBase<TEntity, TKey, TReadModel,
        TModel, TFilteredPagedRequest>
        where TEntity : Entity<TKey>, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedRequest : class, IFilteredPagedRequest
        where TKey : IEquatable<TKey>
    {
        protected ISession Session { get; }
        protected IQueryable<TEntity> EntitySet { get; }

        protected EntityService(ISession session, IEventBus bus) : base(bus)
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

        protected sealed override async Task<IPagedResult<TEntity>> FindEntityPagedListAsync(IPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return await FindEntityQueryable.ToPagedListAsync(request, cancellationToken);
        }

        protected sealed override Task CreateEntityListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            return Session.SaveAsync(entityList, cancellationToken);
        }

        protected sealed override Task UpdateEntityListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            return Session.UpdateAsync(entityList, cancellationToken);
        }

        protected sealed override Task RemoveEntityListAsync(IReadOnlyList<TEntity> entityList,
            CancellationToken cancellationToken)
        {
            return Session.DeleteAsync(entityList, cancellationToken);
        }
    }
}