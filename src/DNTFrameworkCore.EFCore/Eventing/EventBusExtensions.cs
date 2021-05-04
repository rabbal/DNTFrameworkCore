using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Eventing;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DNTFrameworkCore.EFCore.Eventing
{
    public static class EventBusExtensions
    {
        public static async Task DispatchDomainEvents(this IEventBus bus, IEnumerable<EntityEntry> entries)
        {
            var domainEntities = entries.Select(entry => entry.Entity)
                .OfType<IAggregateRoot>()
                .Where(x => x.DomainEvents != null && x.DomainEvents.Any()).ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            domainEntities
                .ForEach(entity => entity.EmptyDomainEvents());

            foreach (var domainEvent in domainEvents)
                await bus.Dispatch(domainEvent);
        }
    }
}