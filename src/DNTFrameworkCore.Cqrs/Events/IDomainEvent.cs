using MediatR;

namespace DNTFrameworkCore.Cqrs.Events
{
    public interface IDomainEvent : INotification
    {
    }

    public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : INotification
    {
    }
}