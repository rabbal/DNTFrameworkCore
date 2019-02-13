using System.Threading.Tasks;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Eventing
{
    [TestFixture]
    public class EventBusTests
    {
        [Test]
        public async Task Should_Trigger_With_Multiple_Handler_That_Returns_Ok_Result()
        {
            var services = new ServiceCollection();
            services.AddDNTFramework();
            services.AddTransient<IDomainEventHandler<SimpleDomainEvent>, SimpleDomainEventHandler1>();
            services.AddTransient<IDomainEventHandler<SimpleDomainEvent>, SimpleDomainEventHandler2>();
            var bus = services.BuildServiceProvider().GetRequiredService<IEventBus>();

            var result = await bus.TriggerAsync(new SimpleDomainEvent("TestValue"));
            result.Succeeded.ShouldBeTrue();
            SimpleDomainEventHandler1.HandleCount.ShouldBe(1);
            SimpleDomainEventHandler2.HandleCount.ShouldBe(1);
        }

        [Test]
        public async Task Should_Failed_Trigger_When_One_Handler_Failed()
        {
            var services = new ServiceCollection();
            services.AddDNTFramework();
            services.AddTransient<IDomainEventHandler<SimpleDomainEvent>, FailedSimpleDomainEventHandler>();
            var bus = services.BuildServiceProvider().GetRequiredService<IEventBus>();

            var result = await bus.TriggerAsync(new SimpleDomainEvent("TestValue"));
            result.Succeeded.ShouldBeFalse();
            FailedSimpleDomainEventHandler.HandleCount.ShouldBe(1);
            result.Message.ShouldBe(nameof(FailedSimpleDomainEventHandler));
        }

        private class SimpleDomainEventHandler1 : IDomainEventHandler<SimpleDomainEvent>
        {
            public static int HandleCount { get; set; }

            public Task<Result> Handle(SimpleDomainEvent domainEvent)
            {
                ++HandleCount;
                return Task.FromResult(Result.Ok());
            }
        }

        private class SimpleDomainEventHandler2 : IDomainEventHandler<SimpleDomainEvent>
        {
            public static int HandleCount { get; set; }

            public Task<Result> Handle(SimpleDomainEvent domainEvent)
            {
                ++HandleCount;
                return Task.FromResult(Result.Ok());
            }
        }

        private class FailedSimpleDomainEventHandler : IDomainEventHandler<SimpleDomainEvent>
        {
            public static int HandleCount { get; set; }

            public Task<Result> Handle(SimpleDomainEvent domainEvent)
            {
                ++HandleCount;
                return Task.FromResult(Result.Failed(nameof(FailedSimpleDomainEventHandler)));
            }
        }

        private class SimpleDomainEvent : IDomainEvent
        {
            public SimpleDomainEvent(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }
}