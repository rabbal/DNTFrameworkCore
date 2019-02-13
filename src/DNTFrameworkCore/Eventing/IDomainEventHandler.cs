using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Eventing
{
    public interface IDomainEventHandler<in T> : ITransientDependency
        where T : IDomainEvent
    {
        Task<Result> Handle(T domainEvent);
    }
}