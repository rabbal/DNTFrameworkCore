using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Auditing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace DNTFrameworkCore.Tests.Auditing
{
    [TestFixture]
    public class AuditingInterceptorTests
    {
        [Test]
        public void Should_Intercept_IApplicationService_Sync_Method()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFramework()
                .AddAuditingOptions(options =>
                {
                    options.Enabled = true;
                    options.EnabledForAnonymousUsers = true;
                });
            services.AddScoped<IPartyService, PartyService>();
            services.Replace(ServiceDescriptor.Describe(typeof(IAuditingStore), typeof(StubAuditingStore),
                ServiceLifetime.Scoped));
            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<AuditingInterceptor>()));

            services.AddLogging();

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();
            Assert.Throws<Exception>(() => service.SyncMethod(), "StubAuditingStore.Save");
        }

        [Test]
        public void Should_Intercept_IApplicationService_Async_Method()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFramework()
                .AddAuditingOptions(options =>
                {
                    options.Enabled = true;
                    options.EnabledForAnonymousUsers = true;
                });
            services.AddScoped<IPartyService, PartyService>();
            services.Replace(ServiceDescriptor.Describe(typeof(IAuditingStore), typeof(StubAuditingStore),
                ServiceLifetime.Scoped));
            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<AuditingInterceptor>()));

            services.AddLogging();

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();
            Assert.ThrowsAsync<Exception>(() => service.AsyncMethod(), "StubAuditingStore.Save");
        }


        private class StubAuditingStore : IAuditingStore
        {
            public void Save(AuditInfo info)
            {
                throw new Exception("StubAuditingStore.Save");
            }
        }
    }

    public interface IPartyService : IApplicationService
    {
        void SyncMethod();
        Task AsyncMethod();
    }

    public class PartyService : ApplicationService, IPartyService
    {
        public void SyncMethod()
        {
        }

        public Task AsyncMethod()
        {
            return Task.CompletedTask;
        }
    }
}