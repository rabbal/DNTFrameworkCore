namespace ProjectName.IntegrationTests.Stubs
{
    public class StubEventBus : IEventBus
    {

        public Task<Result> TriggerAsync(IBusinessEvent businessEvent)
        {
            return Task.FromResult(Result.Ok());
        }

        public Task TriggerAsync(IDomainEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}