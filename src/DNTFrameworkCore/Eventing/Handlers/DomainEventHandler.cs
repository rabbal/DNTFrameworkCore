using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Eventing.Handlers
{
    internal abstract class DomainEventHandler
    {
        public abstract Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken,
            IServiceProvider serviceFactory,
            Func<IEnumerable<Func<IDomainEvent, CancellationToken, Task>>, IDomainEvent, CancellationToken, Task>
                dispatch);
    }

    internal class DomainEventHandlerImpl<TDomainEvent> : DomainEventHandler where TDomainEvent : IDomainEvent
    {
        public override Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken,
            IServiceProvider serviceFactory,
            Func<IEnumerable<Func<IDomainEvent, CancellationToken, Task>>, IDomainEvent, CancellationToken, Task>
                dispatch)
        {
            var handlers = serviceFactory.GetServices<IDomainEventHandler<TDomainEvent>>()
                .Select(handler => new Func<IDomainEvent, CancellationToken, Task>(
                    (theDomainEvent, theCancellationToken) =>
                        handler.Handle((TDomainEvent)theDomainEvent, theCancellationToken)));


            return dispatch(handlers, domainEvent, cancellationToken);
        }
    }
}