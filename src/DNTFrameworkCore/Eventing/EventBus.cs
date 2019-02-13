using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Functional;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Eventing
{
    public interface IEventBus : IScopedDependency
    {
        Task<Result> TriggerAsync<T>(T domainEvent) where T : IDomainEvent;
    }

    internal class EventBus : IEventBus
    {
        private readonly IServiceProvider _provider;

        public EventBus(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<Result> TriggerAsync<T>(T domainEvent) where T : IDomainEvent
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            foreach (var handler in _provider.GetServices(handlerType))
            {
                var method = handlerType.GetMethod(
                    nameof(IDomainEventHandler<T>.Handle),
                    new[] {eventType}
                );

                if (method == null) continue;

                var result = await (Task<Result>) method.Invoke(handler, new object[] {domainEvent});

                if (!result.Succeeded)
                    return result;
            }

            return Result.Ok();
        }
    }
}