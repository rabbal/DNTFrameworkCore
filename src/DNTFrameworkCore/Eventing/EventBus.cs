using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Eventing
{
    public interface IEventBus : IScopedDependency
    {
        Task<Result> TriggerAsync(IBusinessEvent businessEvent);
        Task TriggerAsync(IDomainEvent domainEvent);
    }

    internal class EventBus : IEventBus
    {
        private const string MethodName = "Handle";
        private readonly IServiceProvider _provider;

        public EventBus(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<Result> TriggerAsync(IBusinessEvent businessEvent)
        {
            var eventType = businessEvent.GetType();
            var handlerType = typeof(IBusinessEventHandler<>).MakeGenericType(eventType);

            foreach (var handler in _provider.GetServices(handlerType))
            {
                var method = handlerType.GetMethod(MethodName, new[] {eventType});

                if (method == null) continue;

                var result = await (Task<Result>) method.Invoke(handler, new object[] {businessEvent});

                if (result.Failed) return result;
            }

            return Result.Ok();
        }

        public async Task TriggerAsync(IDomainEvent domainEvent)
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            foreach (var handler in _provider.GetServices(handlerType))
            {
                var method = handlerType.GetMethod(MethodName, new[] {eventType});

                if (method == null) continue;

                await (Task) method.Invoke(handler, new object[] {domainEvent});
            }
        }
    }
}