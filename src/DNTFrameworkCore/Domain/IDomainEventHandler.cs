using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Domain
{
    public interface IDomainEventHandler<in T> : ITransientDependency
        where T : IDomainEvent
    {
        Task Handle(T domainEvent);
    }

    public abstract class DomainEventHandler<T> : IDomainEventHandler<T> where T : IDomainEvent
    {
        public abstract Task Handle(T domainEvent);
    }
}