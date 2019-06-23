using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Events;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.NHibernate.Linq;
using DNTFrameworkCore.Transaction;
using DNTFrameworkCore.Validation;
using NHibernate;
using NHibernate.Linq;

namespace DNTFrameworkCore.NHibernate.Application
{
    public abstract class CrudService<TEntity, TKey, TReadModel, TModel,
        TFilteredPagedQueryModel> : ApplicationService,
        ICrudService<TKey, TReadModel, TModel, TFilteredPagedQueryModel>
        where TEntity : Entity<TKey>, ITrackable, new()
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel
        where TKey : IEquatable<TKey>
    {
        protected ISession Session { get; }
        protected IEventBus EventBus { get; }
        protected IQueryable<TEntity> EntitySet { get; }

        protected CrudService(ISession session, IEventBus bus)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
            EventBus = bus ?? throw new ArgumentNullException(nameof(bus));

            EntitySet = session.Query<TEntity>();
        }

        [SkipValidation]
        public async Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel model)
        {
            return await BuildReadQuery(model).ToPagedQueryResultAsync(model);
        }

        public async Task<Maybe<TModel>> FindAsync(TKey id)
        {
            var models = await FindAsync(BuildEqualityExpressionForId(id));

            return models.SingleOrDefault();
        }

        public Task<IReadOnlyList<TModel>> FindListAsync(IEnumerable<TKey> ids)
        {
            return FindAsync(entity => ids.Contains(entity.Id));
        }

        public Task<IReadOnlyList<TModel>> FindListAsync()
        {
            return FindAsync(_ => true);
        }

        [SkipValidation]
        public async Task<IPagedQueryResult<TModel>> FindPagedListAsync(PagedQueryModel model)
        {
            var pagedList = await BuildFindQuery()
                .ToPagedQueryResultAsync(model);

            var result = new PagedQueryResult<TModel>
            {
                Items = pagedList.Items.MapReadOnlyList(MapToModel),
                TotalCount = pagedList.TotalCount
            };

            await AfterFindAsync(result.Items);

            return result;
        }

        [Transactional]
        public Task<Result> CreateAsync(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return CreateAsync(new[] {model});
        }

        [Transactional]
        public async Task<Result> CreateAsync(IEnumerable<TModel> models)
        {
            var modelList = models.ToList();

            var result = await BeforeCreateAsync(modelList);
            if (result.Failed) return result;

            var entityList = modelList.MapReadOnlyList<TModel, TEntity>(MapToEntity);

            await AfterMappingAsync(modelList, entityList);

            result = await EventBus.TriggerCreatingEventAsync<TModel, TKey>(modelList);
            if (result.Failed) return result;

            await Session.SaveAsync(entityList);

            MapToModel(entityList, modelList);

            result = await AfterCreateAsync(modelList);
            if (result.Failed) return result;

            result = await EventBus.TriggerCreatedEventAsync<TModel, TKey>(modelList);

            return result;
        }

        [Transactional]
        public Task<Result> EditAsync(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return EditAsync(new[] {model});
        }

        [Transactional]
        public async Task<Result> EditAsync(IEnumerable<TModel> models)
        {
            var modelList = models.ToList();

            var ids = modelList.Select(m => m.Id).ToList();
            var entityList = await BuildFindQuery().Where(e => ids.Contains(e.Id)).ToListAsync();

            var modifiedList = BuildModifiedModel(modelList, entityList);

            var result = await BeforeEditAsync(modifiedList, entityList);
            if (result.Failed) return result;

            MapToEntity(modelList, entityList);

            await AfterMappingAsync(modelList, entityList);

            result = await EventBus.TriggerEditingEventAsync<TModel, TKey>(modifiedList);
            if (result.Failed) return result;

            entityList.ForEach(e => e.TrackingState = TrackingState.Modified);
            await Session.UpdateAsync(entityList);

            MapToModel(entityList, modelList);

            result = await AfterEditAsync(modifiedList, entityList);
            if (result.Failed) return result;

            result = await EventBus.TriggerEditedEventAsync<TModel, TKey>(modifiedList);

            return result;
        }

        [Transactional]
        [SkipValidation]
        public Task<Result> DeleteAsync(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return DeleteAsync(new[] {model});
        }

        [Transactional]
        [SkipValidation]
        public virtual async Task<Result> DeleteAsync(IEnumerable<TModel> models)
        {
            var modelList = models.ToList();

            var result = await BeforeDeleteAsync(modelList);
            if (result.Failed) return result;

            var entityList = modelList.MapReadOnlyList<TModel, TEntity>(MapToEntity);

            result = await EventBus.TriggerDeletingEventAsync<TModel, TKey>(modelList);
            if (result.Failed) return result;

            await Session.DeleteAsync(entityList);

            result = await AfterDeleteAsync(modelList);
            if (result.Failed) return result;

            result = await EventBus.TriggerDeletedEventAsync<TModel, TKey>(modelList);

            return result;
        }

        [Transactional]
        [SkipValidation]
        public async Task<Result> DeleteAsync(TKey id)
        {
            var model = await FindAsync(id);
            if (model.HasValue) return await DeleteAsync(model.Value);

            return Ok();
        }

        [Transactional]
        [SkipValidation]
        public async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await FindListAsync(ids);
            if (models.Any()) return await DeleteAsync(models);

            return Ok();
        }

        public Task<bool> ExistsAsync(TKey id)
        {
            return EntitySet.AnyAsync(BuildEqualityExpressionForId(id));
        }

        protected abstract IQueryable<TReadModel> BuildReadQuery(TFilteredPagedQueryModel model);

        protected async Task<IReadOnlyList<TModel>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entityList = await BuildFindQuery()
                .Where(predicate)
                .ToListAsync();

            var modelList = entityList.MapReadOnlyList(MapToModel);

            await AfterFindAsync(modelList);

            return modelList;
        }

        protected virtual IQueryable<TEntity> BuildFindQuery()
        {
            return Session.Query<TEntity>();
        }

        protected virtual Task AfterFindAsync(IReadOnlyList<TModel> models)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterMappingAsync(IReadOnlyList<TModel> models, IReadOnlyList<TEntity> entities)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<Result> BeforeCreateAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterCreateAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> BeforeEditAsync(
            IReadOnlyList<ModifiedModel<TModel>> models, IReadOnlyList<TEntity> entities)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterEditAsync(
            IReadOnlyList<ModifiedModel<TModel>> models, IReadOnlyList<TEntity> entities)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> BeforeDeleteAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterDeleteAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected abstract void MapToEntity(TModel model, TEntity entity);

        protected abstract TModel MapToModel(TEntity entity);

        private IReadOnlyList<ModifiedModel<TModel>> BuildModifiedModel(IReadOnlyCollection<TModel> models,
            IReadOnlyCollection<TEntity> entities)
        {
            if (models.Count != entities.Count) throw new ConcurrencyException();

            var modelList = entities.MapReadOnlyList(MapToModel);
            var modelDictionary = modelList.ToDictionary(e => e.Id);

            var result = models.Select(
                model => new ModifiedModel<TModel>
                    {NewValue = model, OriginalValue = modelDictionary[model.Id]}).ToList();

            return result;
        }

        private void MapToModel(IReadOnlyList<TEntity> entities, IEnumerable<TModel> models)
        {
            var i = 0;
            foreach (var model in models)
            {
                var entity = entities[i++];
                var m = MapToModel(entity);

                var properties = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite).ToList();
                foreach (var property in properties) property.SetValue(model, property.GetValue(m));
            }
        }

        private void MapToEntity(IEnumerable<TModel> models, IReadOnlyList<TEntity> entities)
        {
            var i = 0;
            foreach (var model in models)
            {
                var entity = entities[i++];
                MapToEntity(model, entity);
            }
        }

        private Expression<Func<TEntity, bool>> BuildEqualityExpressionForId(TKey id)
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