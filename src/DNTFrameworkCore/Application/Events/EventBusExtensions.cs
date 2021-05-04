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
        public static Task<Result> DispatchCreatingEvent<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return Dispatch(bus, typeof(CreatingBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> DispatchCreatedEvent<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return Dispatch(bus, typeof(CreatedBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> DispatchEditingEvent<TModel, TKey>(this IEventBus bus,
            IEnumerable<ModifiedModel<TModel>> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return Dispatch(bus, typeof(EditingBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> DispatchEditedEvent<TModel, TKey>(this IEventBus bus,
            IEnumerable<ModifiedModel<TModel>> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return Dispatch(bus, typeof(EditedBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> DispatchDeletingEvent<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return Dispatch(bus, typeof(DeletingBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        public static Task<Result> DispatchDeletedEvent<TModel, TKey>(this IEventBus bus,
            IEnumerable<TModel> models, CancellationToken cancellationToken = default)
            where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
        {
            return Dispatch(bus, typeof(DeletedBusinessEvent<TModel, TKey>), models, cancellationToken);
        }

        private static Task<Result> Dispatch(IEventBus bus, Type eventType, object model,
            CancellationToken cancellationToken = default)
        {
            var businessEvent = (IBusinessEvent)Activator.CreateInstance(eventType, model);

            return bus.Dispatch(businessEvent, cancellationToken);
        }
    }
}