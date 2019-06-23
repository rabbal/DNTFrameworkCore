using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation.Interception;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using ValidationException = DNTFrameworkCore.Exceptions.ValidationException;

namespace DNTFrameworkCore.Tests.Validation
{
    [TestFixture]
    public class ValidationInterceptorTests
    {
        [Test]
        public void Should_Intercept_IApplicationService_Sync_Method_Return_Result()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore()
                .AddModelValidation();

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<ValidationInterceptor>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            var result = service.SyncMethod(new PartyModel());
            result.Failed.ShouldBeTrue();
            result.Failures.Any(validationResult => validationResult.MemberName == nameof(PartyModel.DisplayName));
        }

        [Test]
        public async Task Should_Intercept_IApplicationService_Async_Method_Return_TaskResult()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore()
                .AddModelValidation();

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<ValidationInterceptor>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            var result = await service.AsyncMethod(new PartyModel());
            result.Failed.ShouldBeTrue();
            result.Failures.Any(validationResult => validationResult.MemberName == nameof(PartyModel.DisplayName));
        }

        [Test]
        public void Should_Intercept_IApplicationService_Sync_Method_Return_Void()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore()
                .AddModelValidation();

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<ValidationInterceptor>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            var exception = Assert.Throws<ValidationException>(() => service.VoidSyncMethod(new PartyModel()));
            exception.ShouldNotBeNull();
            exception.Failures.Any(validationResult => validationResult.MemberName == nameof(PartyModel.DisplayName));
        }

        [Test]
        public void Should_Intercept_IApplicationService_Async_Method_Return_Task()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore()
                .AddModelValidation();

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<ValidationInterceptor>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            var exception =
                Assert.ThrowsAsync<ValidationException>(async () =>
                    await service.TaskAsyncMethod(new PartyModel()));
            exception.ShouldNotBeNull();

            exception.Failures.Any(validationResult => validationResult.MemberName == nameof(PartyModel.DisplayName));
        }
    }

    public class PartyModel
    {
        [Required] public string DisplayName { get; set; }
    }

    public interface IPartyService : IApplicationService
    {
        Result SyncMethod(PartyModel model);
        Task<Result> AsyncMethod(PartyModel model);
        void VoidSyncMethod(PartyModel model);
        Task TaskAsyncMethod(PartyModel model);
    }

    public class PartyService : ApplicationService, IPartyService
    {
        public Result SyncMethod(PartyModel model)
        {
            return Result.Ok();
        }

        public Task<Result> AsyncMethod(PartyModel model)
        {
            return Task.FromResult(Result.Ok());
        }

        public void VoidSyncMethod(PartyModel model)
        {
        }

        public Task TaskAsyncMethod(PartyModel model)
        {
            return Task.CompletedTask;
        }
    }
}