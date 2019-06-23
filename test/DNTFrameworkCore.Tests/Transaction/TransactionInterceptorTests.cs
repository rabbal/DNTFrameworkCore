using System;
using System.Data;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Transaction;
using DNTFrameworkCore.Transaction.Interception;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;

namespace DNTFrameworkCore.Tests.Transaction
{
    [TestFixture]
    public class TransactionInterceptorTests
    {
        [Test]
        public void Should_Intercept_As_AmbientTransaction()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object);
            transactionProviderMock.SetupGet(transactionProvider =>
                transactionProvider.CurrentTransaction).Returns(transactionMock.Object);

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            service.VoidSyncMethod();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted),
                Times.Never);
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction, Times.Once);
            transactionMock.Verify(transaction => transaction.Commit(), Times.Never);
            transactionMock.Verify(transaction => transaction.Rollback(), Times.Never);
        }

        [Test]
        public void Should_Not_Intercept_Method_When_Method_Has_Not_Transactional_Attribute()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            service.MethodWithoutTransactionalAttribute();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted),
                Times.Never);
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction, Times.Never);
            transactionMock.Verify(transaction => transaction.Commit(), Times.Never);
            transactionMock.Verify(transaction => transaction.Rollback(), Times.Never);
        }

        [Test]
        public void Should_Intercept_And_Commit_Transaction_On_Sync_Method_That_Return_Ok_Result()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            service.SyncMethodWithOkResult();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Commit());
        }

        [Test]
        public async Task Should_Intercept_And_Commit_Transaction_On_Async_Method_That_Return_Ok_TaskResult()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            await service.AsyncMethodWithOkResult();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Commit());
        }

        [Test]
        public void Should_Intercept_And_Rollback_Transaction_On_Sync_Method_That_Return_Failed_Result()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            service.SyncMethodWithFailedResult();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Rollback());
        }

        [Test]
        public async Task Should_Intercept_And_Rollback_Transaction_On_Async_Method_That_Return_Failed_TaskResult()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            await service.AsyncMethodWithFailedResult();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Rollback());
        }

        [Test]
        public void Should_Intercept_And_Commit_Transaction_On_Sync_Method_That_Return_Void()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            service.VoidSyncMethod();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Commit());
        }

        [Test]
        public async Task Should_Intercept_And_Commit_Transaction_On_Async_Method_That_Return_Task()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            await service.TaskAsyncMethod();
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Commit());
        }

        [Test]
        public void Should_Intercept_And_Rollback_Transaction_On_Sync_Method_That_Throw_Exception()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();

            Assert.Throws<Exception>(() => { service.VoidSyncMethodWithException(); },
                nameof(IPartyService.VoidSyncMethodWithException));
            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Rollback());
        }

        [Test]
        public void Should_Intercept_And_Rollback_Transaction_On_Async_Method_That_Throw_Exception()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            services.AddDNTFrameworkCore();

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(transaction => transaction.Commit());
            transactionMock.Setup(transaction => transaction.Rollback());

            var transactionProviderMock = new Mock<ITransactionProvider>();

            transactionProviderMock.Setup(transactionProvider =>
                    transactionProvider.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => transactionMock.Object).Callback(() =>
                {
                    transactionProviderMock.SetupGet(transactionProvider =>
                        transactionProvider.CurrentTransaction).Returns(transactionMock.Object);
                });

            services.Replace(ServiceDescriptor.Scoped(provider => transactionProviderMock.Object));

            services.AddScoped<IPartyService, PartyService>();

            services.Decorate<IPartyService>((target, provider) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    provider.GetService<TransactionInterceptorBase>()));

            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();
            Assert.ThrowsAsync<Exception>(async () => { await service.TaskAsyncMethodWithException(); },
                nameof(IPartyService.TaskAsyncMethodWithException));

            transactionProviderMock.Verify(transaction => transaction.BeginTransaction(IsolationLevel.ReadCommitted));
            transactionProviderMock.Verify(transaction => transaction.CurrentTransaction);
            transactionMock.Verify(transaction => transaction.Rollback());
        }
    }


    public interface IPartyService : IApplicationService
    {
        void MethodWithoutTransactionalAttribute();
        Result SyncMethodWithOkResult();
        Result SyncMethodWithFailedResult();
        Task<Result> AsyncMethodWithOkResult();
        Task<Result> AsyncMethodWithFailedResult();
        void VoidSyncMethod();
        void VoidSyncMethodWithException();
        Task TaskAsyncMethod();
        Task TaskAsyncMethodWithException();
    }

    public class PartyService : ApplicationService, IPartyService
    {
        public void MethodWithoutTransactionalAttribute()
        {
        }

        [Transactional]
        public Result SyncMethodWithOkResult()
        {
            return Result.Ok();
        }

        [Transactional]
        public Result SyncMethodWithFailedResult()
        {
            return Result.Fail("failure message");
        }

        [Transactional]
        public Task<Result> AsyncMethodWithOkResult()
        {
            return Task.FromResult(Result.Ok());
        }

        [Transactional]
        public Task<Result> AsyncMethodWithFailedResult()
        {
            return Task.FromResult(Result.Fail("failure message"));
        }

        [Transactional]
        public void VoidSyncMethod()
        {
        }

        [Transactional]
        public void VoidSyncMethodWithException()
        {
            throw new Exception(nameof(VoidSyncMethodWithException));
        }

        [Transactional]
        public Task TaskAsyncMethod()
        {
            return Task.CompletedTask;
        }

        [Transactional]
        public Task TaskAsyncMethodWithException()
        {
            throw new Exception(nameof(TaskAsyncMethodWithException));
        }
    }
}