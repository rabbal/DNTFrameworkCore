using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Functional;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Eventing
{
    public interface IEventBus : IScopedDependency
    {
        Task<Result> TriggerAsync<T>(T @event) where T : IBusinessEvent;
    }

    internal class EventBus : IEventBus
    {
        private readonly IServiceProvider _provider;

        public EventBus(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<Result> TriggerAsync<T>(T @event) where T : IBusinessEvent
        {
            var eventType = @event.GetType();
            var handlerType = typeof(IBusinessEventHandler<>).MakeGenericType(eventType);

            foreach (var handler in _provider.GetServices(handlerType))
            {
                var method = handlerType.GetMethod(
                    nameof(IBusinessEventHandler<T>.Handle),
                    new[] { eventType }
                );

                if (method == null) continue;

                var result = await (Task<Result>)method.Invoke(handler, new object[] { @event });

                if (!result.Succeeded) return result;
            }

            return Result.Ok();
        }
    }
}