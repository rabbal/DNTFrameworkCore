using MediatR;
using System;

namespace DNTFrameworkCore.Cqrs.Events
{
    public interface IIntegrationEvent : INotification
    {
        Guid Id { get; }
        DateTimeOffset Timestamp { get; }
    }
}
