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
            services.AddDNTFrameworkCore();
            services.AddTransient<IBusinessEventHandler<SimpleBusinessEvent>, SimpleBusinessEventHandler1>();
            services.AddTransient<IBusinessEventHandler<SimpleBusinessEvent>, SimpleBusinessEventHandler2>();
            var bus = services.BuildServiceProvider().GetRequiredService<IEventBus>();

            var result = await bus.TriggerAsync(new SimpleBusinessEvent("TestValue"));
            result.Failed.ShouldBeFalse();
            SimpleBusinessEventHandler1.HandleCount.ShouldBe(1);
            SimpleBusinessEventHandler2.HandleCount.ShouldBe(1);
        }

        [Test]
        public async Task Should_Failed_Trigger_When_One_Handler_Failed()
        {
            var services = new ServiceCollection();
            services.AddDNTFrameworkCore();
            services.AddTransient<IBusinessEventHandler<SimpleBusinessEvent>, FailedSimpleBusinessEventHandler>();
            var bus = services.BuildServiceProvider().GetRequiredService<IEventBus>();

            var result = await bus.TriggerAsync(new SimpleBusinessEvent("TestValue"));
            result.Failed.ShouldBeTrue();
            FailedSimpleBusinessEventHandler.HandleCount.ShouldBe(1);
            result.Message.ShouldBe(nameof(FailedSimpleBusinessEventHandler));
        }

        private class SimpleBusinessEventHandler1 : IBusinessEventHandler<SimpleBusinessEvent>
        {
            public static int HandleCount { get; set; }

            public Task<Result> Handle(SimpleBusinessEvent @event)
            {
                ++HandleCount;
                return Task.FromResult(Result.Ok());
            }
        }

        private class SimpleBusinessEventHandler2 : IBusinessEventHandler<SimpleBusinessEvent>
        {
            public static int HandleCount { get; set; }

            public Task<Result> Handle(SimpleBusinessEvent @event)
            {
                ++HandleCount;
                return Task.FromResult(Result.Ok());
            }
        }

        private class FailedSimpleBusinessEventHandler : IBusinessEventHandler<SimpleBusinessEvent>
        {
            public static int HandleCount { get; set; }

            public Task<Result> Handle(SimpleBusinessEvent @event)
            {
                ++HandleCount;
                return Task.FromResult(Result.Fail(nameof(FailedSimpleBusinessEventHandler)));
            }
        }

        private class SimpleBusinessEvent : IBusinessEvent
        {
            public SimpleBusinessEvent(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }
}