using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Application
{
    public static class EventBusExtensions
    {
        public static Task<Result> TriggerCreatingEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(CreatingBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> TriggerCreatedEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(CreatedBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> TriggerEditingEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<ModifiedModel<TModel>> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(EditingBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> TriggerEditedEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<ModifiedModel<TModel>> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(EditedBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> TriggerDeletingEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(DeletingBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> TriggerDeletedEventAsync<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return TriggerAsync(bus, typeof(DeletedBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        private static Task<Result> TriggerAsync(IEventBus bus, Type eventType, object model,
            CancellationToken cancellationToken = default)
        {
            var businessEvent = (IBusinessEvent) Activator.CreateInstance(eventType, model);

            return bus.TriggerAsync(businessEvent, cancellationToken);
        }
    }
}