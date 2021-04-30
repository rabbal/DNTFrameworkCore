using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Domain
{
    public interface IDomainEventHandler<in T> : ITransientDependency
        where T : IDomainEvent
    {
        Task Handle(T domainEvent, CancellationToken cancellationToken = default);
    }
}