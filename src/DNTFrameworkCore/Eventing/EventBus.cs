using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Common;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Eventing.Handlers;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Eventing
{
    public interface IEventBus : IScopedDependency
    {
        Task<Result> Dispatch(IBusinessEvent businessEvent, CancellationToken cancellationToken = default);
        Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
        // TODO: Implement IntegrationEvent dispatching mechanism
        // Task Publish(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
        // ISubscription Subscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>;
        // ISubscription SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;
    }

    public class EventBus : IEventBus
    {
        private static readonly LockingConcurrentDictionary<Type, DomainEventHandler> _domainEventHandlers = new();
        private static readonly LockingConcurrentDictionary<Type, BusinessEventHandler> _businessEventHandlers = new();

        private readonly IServiceProvider _provider;

        public EventBus(IServiceProvider provider) => _provider = provider;

        public Task<Result> Dispatch(IBusinessEvent businessEvent,
            CancellationToken cancellationToken = default)
        {
            if (businessEvent == null) throw new ArgumentNullException(nameof(businessEvent));

            return DispatchBusinessEvent(businessEvent, cancellationToken);
        }

        public Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

            return DispatchDomainEvent(domainEvent, cancellationToken);
        }

        protected virtual async Task<Result> DispatchInternal(
            IEnumerable<Func<IBusinessEvent, CancellationToken, Task<Result>>> handlers,
            IBusinessEvent businessEvent, CancellationToken cancellationToken = default)
        {
            foreach (var handle in handlers)
            {
                var result = await handle(businessEvent, cancellationToken);
                if (result.Failed) return result;
            }

            return Result.Ok();
        }

        protected virtual Task DispatchInternal(IEnumerable<Func<IDomainEvent, CancellationToken, Task>> handlers,
            IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handles = handlers.Select(handle => handle(domainEvent, cancellationToken)).ToList();

            return Task.WhenAll(handles);
        }

        private Task<Result> DispatchBusinessEvent(IBusinessEvent businessEvent,
            CancellationToken cancellationToken = default)
        {
            var domainEventType = businessEvent.GetType();
            var handlers = _businessEventHandlers.GetOrAdd(domainEventType,
                static type => (BusinessEventHandler) Activator.CreateInstance(typeof(BusinessEventHandlerImpl<>)
                                   .MakeGenericType(type)) ??
                               throw new InvalidOperationException($"Could not create handler for type {type}"));

            return handlers.Handle(businessEvent, cancellationToken, _provider, DispatchInternal);
        }

        private Task DispatchDomainEvent(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var domainEventType = domainEvent.GetType();
            var handlers = _domainEventHandlers.GetOrAdd(domainEventType,
                static type => (DomainEventHandler) Activator.CreateInstance(typeof(DomainEventHandlerImpl<>)
                                   .MakeGenericType(type)) ??
                               throw new InvalidOperationException($"Could not create handler for type {type}"));

            return handlers.Handle(domainEvent, cancellationToken, _provider, DispatchInternal);
        }
    }
}