using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Events;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.EntityFramework.Context.Extensions;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Transaction;
using DNTFrameworkCore.Validation;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Application
{
    public abstract class CrudService<TEntity, TKey, TModel> :
        CrudService<TEntity, TKey, TModel, TModel>,
        ICrudService<TKey, TModel>
        where TEntity : Entity<TKey>, IAggregateRoot, new()
        where TModel : MasterModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected CrudService(CrudServiceDependency dependency) : base(dependency)
        {
        }
    }

    public abstract class CrudService<TEntity, TKey, TReadModel, TModel> :
        CrudService<TEntity, TKey, TReadModel, TModel, FilteredPagedQueryModel>,
        ICrudService<TKey, TReadModel, TModel>
        where TEntity : Entity<TKey>, IAggregateRoot, new()
        where TModel : MasterModel<TKey>
        where TReadModel : MasterModel<TKey>
        where TKey : IEquatable<TKey>
    {
        protected CrudService(CrudServiceDependency dependency) : base(dependency)
        {
        }
    }

    public abstract class CrudService<TEntity, TKey, TReadModel, TModel,
        TFilteredPagedQueryModel> : ApplicationService,
        ICrudService<TKey, TReadModel, TModel, TFilteredPagedQueryModel>
        where TEntity : Entity<TKey>, IAggregateRoot, new()
        where TModel : MasterModel<TKey>
        where TReadModel : MasterModel<TKey>
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel
        where TKey : IEquatable<TKey>
    {
        protected readonly IEventBus EventBus;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly DbSet<TEntity> EntitySet;

        protected CrudService(CrudServiceDependency dependency)
        {
            Guard.ArgumentNotNull(dependency, nameof(dependency));

            UnitOfWork = dependency.UnitOfWork;
            EventBus = dependency.EventBus;
            EntitySet = UnitOfWork.Set<TEntity>();
        }

        [SkipValidation]
        public async Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel model)
        {
            return await BuildReadQuery(model).ToPagedQueryResultAsync(model);
        }

        protected abstract IQueryable<TReadModel> BuildReadQuery(TFilteredPagedQueryModel model);

        public async Task<Maybe<TModel>> FindAsync(TKey id)
        {
            var models = await FindAsync(BuildEqualityExpressionForId(id));

            return models.SingleOrDefault();
        }

        public Task<IReadOnlyList<TModel>> FindAsync(IEnumerable<TKey> ids)
        {
            return FindAsync(entity => ids.Contains(entity.Id));
        }

        public Task<IReadOnlyList<TModel>> FindAsync(params TKey[] ids)
        {
            return FindAsync((IEnumerable<TKey>) ids);
        }

        public Task<IReadOnlyList<TModel>> FindAsync()
        {
            return FindAsync(entity => true);
        }

        [SkipValidation]
        public async Task<IPagedQueryResult<TModel>> FindPagedListAsync(PagedQueryModel model)
        {
            var entityResult = await BuildFindQuery().ToPagedQueryResultAsync(model);

            var result = new PagedQueryResult<TModel>
            {
                Items = MapToModel(entityResult.Items),
                TotalCount = entityResult.TotalCount
            };

            await AfterFindAsync(result.Items);

            return result;
        }

        private async Task<IReadOnlyList<TModel>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = await BuildFindQuery().Where(predicate).ToListAsync();

            var models = MapToModel(entities);

            await AfterFindAsync(models);

            return models;
        }

        protected virtual IQueryable<TEntity> BuildFindQuery()
        {
            return EntitySet.AsNoTracking();
        }

        protected virtual Task AfterFindAsync(IReadOnlyList<TModel> models)
        {
            return Task.CompletedTask;
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
            var modelItems = models.ToList();
            var result = await BeforeCreateAsync(modelItems);
            if (!result.Succeeded) return result;

            var entities = MapToEntity(modelItems);

            await AfterMappingAsync(entities, modelItems);

            result = await EventBus.TriggerCreatingDomainEventAsync<TModel, TKey>(modelItems);
            if (!result.Succeeded) return result;

            UnitOfWork.AddRange(entities);
            await BeforeSaveAsync(entities, modelItems);
            await UnitOfWork.SaveChangesAsync();
            UnitOfWork.AcceptChanges(entities);
            await AfterSaveAsync(entities, modelItems);

            MapToModel(entities, modelItems);

            result = await AfterCreateAsync(modelItems);
            if (!result.Succeeded) return result;

            result = await EventBus.TriggerCreatedDomainEventAsync<TModel, TKey>(modelItems);

            return result;
        }

        protected virtual Task AfterMappingAsync(IReadOnlyList<TEntity> entities, IReadOnlyList<TModel> models)
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

        [Transactional]
        public Task<Result> EditAsync(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return EditAsync(new[] {model});
        }

        [Transactional]
        public async Task<Result> EditAsync(IEnumerable<TModel> models)
        {
            var modelItems = models.ToList();

            var modifieds = await BuildModifiedModelsAsync(modelItems);

            var result = await BeforeEditAsync(modifieds);
            if (!result.Succeeded) return result;

            var entities = MapToEntity(modelItems);

            await AfterMappingAsync(entities, modelItems);

            result = await EventBus.TriggerEditingDomainEventAsync<TModel, TKey>(modifieds);
            if (!result.Succeeded) return result;

            MarkAsModified(entities);

            UnitOfWork.ApplyChanges(entities);
            await BeforeSaveAsync(entities, modelItems);
            await UnitOfWork.SaveChangesAsync();
            UnitOfWork.AcceptChanges(entities);
            await AfterSaveAsync(entities, modelItems);

            MapToModel(entities, modelItems);

            result = await AfterEditAsync(modifieds);
            if (!result.Succeeded) return result;

            result = await EventBus.TriggerEditedDomainEventAsync<TModel, TKey>(modifieds);

            return result;
        }

        private static void MarkAsModified(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.TrackingState = TrackingState.Modified;
            }
        }

        protected virtual Task<Result> BeforeEditAsync(
            IReadOnlyList<ModifiedModel<TModel>> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterEditAsync(
            IReadOnlyList<ModifiedModel<TModel>> models)
        {
            return Task.FromResult(Ok());
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
            var modelItems = models.ToList();

            var result = await BeforeDeleteAsync(modelItems);
            if (!result.Succeeded) return result;

            var entities = MapToEntity(modelItems);

            result = await EventBus.TriggerDeletingDomainEventAsync<TModel, TKey>(modelItems);
            if (!result.Succeeded) return result;

            UnitOfWork.RemoveRange(entities);
            await UnitOfWork.SaveChangesAsync();

            result = await AfterDeleteAsync(modelItems);
            if (!result.Succeeded) return result;

            result = await EventBus.TriggerDeletedDomainEventAsync<TModel, TKey>(modelItems);

            return result;
        }

        [Transactional]
        [SkipValidation]
        public async Task<Result> DeleteAsync(TKey id)
        {
            var model = await FindAsync(id);
            if (model.HasValue)
            {
                return await DeleteAsync(model.Value);
            }

            return Ok();
        }

        [Transactional]
        [SkipValidation]
        public async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await FindAsync(ids);
            if (models.Any())
            {
                return await DeleteAsync(models);
            }

            return Ok();
        }

        protected virtual Task<Result> BeforeDeleteAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        protected virtual Task<Result> AfterDeleteAsync(IReadOnlyList<TModel> models)
        {
            return Task.FromResult(Ok());
        }

        public Task<bool> ExistsAsync(TKey id)
        {
            return EntitySet.AnyAsync(BuildEqualityExpressionForId(id));
        }

        protected abstract TEntity MapToEntity(TModel model);

        protected abstract TModel MapToModel(TEntity entity);

        protected virtual Task BeforeSaveAsync(IReadOnlyList<TEntity> entities, List<TModel> models)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterSaveAsync(IReadOnlyList<TEntity> entities, List<TModel> models)
        {
            return Task.CompletedTask;
        }

        private async Task<IReadOnlyList<ModifiedModel<TModel>>>
            BuildModifiedModelsAsync(IReadOnlyList<TModel> models)
        {
            var originals = (await FindAsync(models.Select(a => a.Id))).OrderBy(a => a.Id)
                .ToDictionary(a => a.Id);

            //TODO: throw exception if count of originals not equals with count of models

            var result = models.Select(
                model => new ModifiedModel<TModel>
                    {NewValue = model, OriginalValue = originals[model.Id]}).ToList();

            return result;
        }

        private IReadOnlyList<TModel> MapToModel(IEnumerable<TEntity> entities)
        {
            return entities.Select(MapToModel).ToList();
        }

        private IReadOnlyList<TEntity> MapToEntity(IEnumerable<TModel> models)
        {
            return models.Select(MapToEntity).ToList();
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
                foreach (var property in properties)
                {
                    property.SetValue(model, property.GetValue(m));
                }
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