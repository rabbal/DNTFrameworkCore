using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;

namespace ProjectName.IntegrationTests.Stubs
{
    public class StubEventBus : IEventBus
    {
        public Task<Result> Dispatch(IBusinessEvent businessEvent, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Ok());
        }

        public Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;   
        }
    }
}