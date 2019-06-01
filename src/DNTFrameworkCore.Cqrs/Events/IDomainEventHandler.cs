using MediatR;

namespace DNTFrameworkCore.Cqrs.Events
{
    public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
    }
}