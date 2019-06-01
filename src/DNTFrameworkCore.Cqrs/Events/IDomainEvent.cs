using MediatR;

namespace DNTFrameworkCore.Cqrs.Events
{
    public interface IDomainEvent : INotification
    {
    }
}