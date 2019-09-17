using MediatR;
using System;

namespace DNTFrameworkCore.Cqrs.Events
{
    public interface IIntegrationEvent : INotification
    {
        Guid Id { get; }
        int Version { get; }
        DateTimeOffset DateTime { get; }
    }
}