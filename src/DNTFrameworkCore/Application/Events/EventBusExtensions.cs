using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Application.Events
{
    public static class EventBusExtensions
    {
        public static Task<Result> TriggerCreatingDomainEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(CreatingDomainEvent<TModel, TKey>), models);
        }

        public static Task<Result> TriggerCreatedDomainEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(CreatedDomainEvent<TModel, TKey>), models);
        }

        public static Task<Result> TriggerEditingDomainEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<ModifiedModel<TModel>> models)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(EditingDomainEvent<TModel, TKey>), models);
        }

        public static Task<Result> TriggerEditedDomainEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<ModifiedModel<TModel>> models)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(EditedDomainEvent<TModel, TKey>), models);
        }
        
        public static Task<Result> TriggerDeletingDomainEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(DeletingDomainEvent<TModel, TKey>), models);
        }

        public static Task<Result> TriggerDeletedDomainEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(DeletedDomainEvent<TModel, TKey>), models);
        }

        private static Task<Result> TriggerAsync(IEventBus bus, Type eventType, object model)
        {
            var domainEvent = (IDomainEvent) Activator.CreateInstance(eventType, model);

            return bus.TriggerAsync(domainEvent);
        }
    }
}