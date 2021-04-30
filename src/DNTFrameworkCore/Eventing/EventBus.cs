using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Common;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Eventing
{
    public interface IEventBus : IScopedDependency
    {
        Task<Result> Publish(IBusinessEvent businessEvent, CancellationToken cancellationToken = default);

        Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
            where TDomainEvent : IDomainEvent;

        Task Publish(object domainEvent, CancellationToken cancellationToken = default);
    }

    public class EventBus : IEventBus
    {
        private static readonly LockingConcurrentDictionary<Type, DomainEventHandlers> _domainEventHandlers = new();
        private static readonly LockingConcurrentDictionary<Type, BusinessEventHandlers> _businessEventHandlers = new();

        private readonly IServiceProvider _serviceFactory;

        public EventBus(IServiceProvider provider) => _serviceFactory = provider;

        public Task<Result> Publish(IBusinessEvent businessEvent,
            CancellationToken cancellationToken = default)
        {
            if (businessEvent == null) throw new ArgumentNullException(nameof(businessEvent));

            return PublishBusinessEvent(businessEvent, cancellationToken);
        }

        public Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
            where TDomainEvent : IDomainEvent
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            return PublishDomainEvent(domainEvent, cancellationToken);
        }

        public Task Publish(object domainEvent, CancellationToken cancellationToken = default)
        {
            return domainEvent switch
            {
                null => throw new ArgumentNullException(nameof(domainEvent)),
                IDomainEvent instance => PublishDomainEvent(instance, cancellationToken),
                _ => throw new ArgumentException($"{nameof(domainEvent)} does not implement ${nameof(IDomainEvent)}")
            };
        }

        protected virtual async Task<Result> PublishInternal(
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

        private Task<Result> PublishBusinessEvent(IBusinessEvent businessEvent,
            CancellationToken cancellationToken = default)
        {
            var domainEventType = businessEvent.GetType();
            var handlers = _businessEventHandlers.GetOrAdd(domainEventType,
                static type => (BusinessEventHandlers) Activator.CreateInstance(typeof(BusinessEventHandlersImpl<>)
                                   .MakeGenericType(type)) ??
                               throw new InvalidOperationException($"Could not create handlers for type {type}"));

            return handlers.Handle(businessEvent, cancellationToken, _serviceFactory, PublishInternal);
        }

        protected virtual Task PublishInternal(IEnumerable<Func<IDomainEvent, CancellationToken, Task>> handlers,
            IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handles = handlers.Select(async handle => await handle(domainEvent, cancellationToken));

            return Task.WhenAll(handles);
        }

        private Task PublishDomainEvent(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var domainEventType = domainEvent.GetType();
            var handlers = _domainEventHandlers.GetOrAdd(domainEventType,
                static type => (DomainEventHandlers) Activator.CreateInstance(typeof(DomainEventHandlersImpl<>)
                                   .MakeGenericType(type)) ??
                               throw new InvalidOperationException($"Could not create handlers for type {type}"));

            return handlers.Handle(domainEvent, cancellationToken, _serviceFactory, PublishInternal);
        }


        private abstract class BusinessEventHandlers
        {
            public abstract Task<Result> Handle(IBusinessEvent businessEvent, CancellationToken cancellationToken,
                IServiceProvider serviceFactory,
                Func<IEnumerable<Func<IBusinessEvent, CancellationToken, Task<Result>>>, IBusinessEvent,
                        CancellationToken, Task<Result>>
                    publish);
        }

        private class BusinessEventHandlersImpl<TBusinessEvent> : BusinessEventHandlers
            where TBusinessEvent : IBusinessEvent
        {
            public override Task<Result> Handle(IBusinessEvent businessEvent, CancellationToken cancellationToken,
                IServiceProvider serviceFactory,
                Func<IEnumerable<Func<IBusinessEvent, CancellationToken, Task<Result>>>, IBusinessEvent,
                        CancellationToken, Task<Result>>
                    publish)
            {
                var handlers = serviceFactory.GetServices<IBusinessEventHandler<TBusinessEvent>>()
                    .Select(handler => new Func<IBusinessEvent, CancellationToken, Task<Result>>(
                        (theBusinessEvent, theCancellationToken) =>
                            handler.Handle((TBusinessEvent) theBusinessEvent, theCancellationToken)));


                return publish(handlers, businessEvent, cancellationToken);
            }
        }

        private abstract class DomainEventHandlers
        {
            public abstract Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken,
                IServiceProvider serviceFactory,
                Func<IEnumerable<Func<IDomainEvent, CancellationToken, Task>>, IDomainEvent, CancellationToken, Task>
                    publish);
        }

        private class DomainEventHandlersImpl<TDomainEvent> : DomainEventHandlers where TDomainEvent : IDomainEvent
        {
            public override Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken,
                IServiceProvider serviceFactory,
                Func<IEnumerable<Func<IDomainEvent, CancellationToken, Task>>, IDomainEvent, CancellationToken, Task>
                    publish)
            {
                var handlers = serviceFactory.GetServices<IDomainEventHandler<TDomainEvent>>()
                    .Select(handler => new Func<IDomainEvent, CancellationToken, Task>(
                        (theDomainEvent, theCancellationToken) =>
                            handler.Handle((TDomainEvent) theDomainEvent, theCancellationToken)));


                return publish(handlers, domainEvent, cancellationToken);
            }
        }
    }
}