using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Eventing
{
    public static class EventBusExtensions
    {
        public static Task Publish(this IEventBus bus, IEnumerable<IDomainEvent> events)
        {
            var tasks = events.Select(async domainEvent => await bus.Publish(domainEvent));
            return Task.WhenAll(tasks);
        }

        public static async Task Publish(this IEventBus bus, IAggregateRoot aggregateRoot)
        {
            await bus.Publish(aggregateRoot.DomainEvents);
            aggregateRoot.EmptyDomainEvents();
        }
    }
}