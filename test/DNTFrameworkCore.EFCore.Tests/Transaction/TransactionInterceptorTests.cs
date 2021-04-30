using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Transaction;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Transaction;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.EFCore.Tests.Transaction
{
    [TestFixture]
    public class TransactionInterceptorTests
    {
        [Test]
        public void Should_Intercept_And_Commit_Transaction_On_Sync_Method_That_Return_Void()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act
            service.VoidSyncMethod();

            //Assert
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransaction());
            dbContext.Verify(context => context.RollbackTransaction(), Times.Never);
        }

        [Test]
        public void Should_Not_Intercept_Method_When_Method_Has_Not_Transactional_Attribute()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act
            service.MethodWithoutTransactionalAttribute();

            //Assert
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never);
            dbContext.Verify(
                context => context.BeginTransactionAsync(IsolationLevel.ReadCommitted, CancellationToken.None),
                Times.Never);
            dbContext.Verify(context => context.CommitTransaction(), Times.Never);
            dbContext.Verify(context => context.RollbackTransaction(), Times.Never);
        }

        [Test]
        public void Should_Intercept_And_Commit_Transaction_On_Sync_Method_That_Return_Ok_Result()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act
            var result = service.SyncMethodWithOkResult();

            //Assert
            result.Failed.ShouldBe(false);
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransaction());
        }

        [Test]
        public async Task Should_Intercept_And_Commit_Transaction_On_Async_Method_That_Return_Ok_TaskResult()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act
            var result = await service.AsyncMethodWithOkResult();

            //Assert
            result.Failed.ShouldBe(false);
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransactionAsync(CancellationToken.None));
        }

        [Test]
        public void Should_Intercept_And_Rollback_Transaction_On_Sync_Method_That_Return_Failed_Result()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act
            var result = service.SyncMethodWithFailedResult();

            //Assert
            result.Failed.ShouldBe(true);
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransaction(), Times.Never);
            dbContext.Verify(context => context.RollbackTransaction());
        }

        [Test]
        public async Task Should_Intercept_And_Rollback_Transaction_On_Async_Method_That_Return_Failed_TaskResult()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act
            var result = await service.AsyncMethodWithFailedResult();

            //Assert
            result.Failed.ShouldBe(true);
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransactionAsync(CancellationToken.None), Times.Never);
            dbContext.Verify(context => context.CommitTransaction(), Times.Never);
            dbContext.Verify(context => context.RollbackTransaction());
        }

        [Test]
        public async Task Should_Intercept_And_Commit_Transaction_On_Async_Method_That_Return_Task()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act
            await service.TaskAsyncMethod();

            //Assert
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransaction(), Times.Never);
            dbContext.Verify(context => context.CommitTransactionAsync(CancellationToken.None));
        }

        [Test]
        public void Should_Intercept_And_Rollback_Transaction_On_Sync_Method_That_Throw_Exception()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act & Assert
            Assert.Throws<Exception>(() => { service.VoidSyncMethodWithException(); },
                nameof(IPartyService.VoidSyncMethodWithException));
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransaction(), Times.Never);
            dbContext.Verify(context => context.CommitTransactionAsync(CancellationToken.None), Times.Never);
            dbContext.Verify(context => context.RollbackTransaction());
        }

        [Test]
        public void Should_Intercept_And_Rollback_Transaction_On_Async_Method_That_Throw_Exception()
        {
            //Arrange
            var (dbContext, service) = ArrangeObjects();

            //Act & Assert
            Assert.ThrowsAsync<Exception>(async () => { await service.TaskAsyncMethodWithException(); },
                nameof(IPartyService.TaskAsyncMethodWithException));
            dbContext.Verify(context => context.BeginTransaction(IsolationLevel.ReadCommitted));
            dbContext.Verify(context => context.CommitTransaction(), Times.Never);
            dbContext.Verify(context => context.CommitTransactionAsync(CancellationToken.None), Times.Never);
            dbContext.Verify(context => context.RollbackTransaction());
        }

        private static (Mock<IDbContext> dbContext, IPartyService service) ArrangeObjects()
        {
            var services = new ServiceCollection();
            var proxyGenerator = new ProxyGenerator();

            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            var dbContextTransaction = new Mock<IDbContextTransaction>();
            var dbContext = new Mock<IDbContext>();

            dbContext.SetupGet(context => context.Transaction).Returns(dbContextTransaction.Object);
            dbContext.Setup(context => context.BeginTransaction(IsolationLevel.ReadCommitted))
                .Returns(() => dbContextTransaction.Object);
            dbContext.Setup(context =>
                    context.BeginTransactionAsync(IsolationLevel.ReadCommitted, CancellationToken.None))
                .Returns(() => Task.FromResult(dbContextTransaction.Object));
            dbContext.SetupGet(context => context.Transaction).Returns(dbContextTransaction.Object);

            services.AddScoped<IPartyService, PartyService>();
            services.Decorate<IPartyService>((target, _) =>
                (IPartyService) proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IPartyService),
                    target,
                    new TransactionInterceptor(dbContext.Object, loggerFactory.Object)));
            var service = services.BuildServiceProvider().GetRequiredService<IPartyService>();
            return (dbContext, service);
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