using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Eventing
{
    public static class EventBusExtensions
    {
        public static Task TriggerAsync(this IEventBus bus, IEnumerable<IDomainEvent> events)
        {
            var tasks = events.Select(async domainEvent => await bus.TriggerAsync(domainEvent));
            return Task.WhenAll(tasks);
        }

        public static async Task PublishAsync(this IEventBus bus, IAggregateRoot aggregateRoot)
        {
            await bus.TriggerAsync(aggregateRoot.Events);
            aggregateRoot.EmptyEvents();
        }
    }
}