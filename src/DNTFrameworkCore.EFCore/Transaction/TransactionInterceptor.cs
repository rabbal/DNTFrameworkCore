using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Threading;
using DNTFrameworkCore.Transaction;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.EFCore.Transaction
{
    public sealed class TransactionInterceptor : IInterceptor
    {
        private readonly IDbContext _dbContext;
        private readonly ILogger _logger;

        public TransactionInterceptor(IDbContext dbContext, ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = Ensure.IsNotNull(loggerFactory, nameof(loggerFactory))
                .CreateLogger("DNTFrameworkCore.Transaction.Interception");
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            var attribute = FindTransactionalAttribute(method);

            //If there is a running transaction, just run the method
            if (attribute == null || _dbContext.HasTransaction)
            {
                invocation.Proceed();
                return;
            }

            _logger.LogInformation("Starting Interception {TypeName}.{MethodName}", invocation.TargetType?.FullName,
                method.Name);

            Intercept(invocation, attribute);

            _logger.LogInformation("Finished Interception {TypeName}.{MethodName}", invocation.TargetType?.FullName,
                method.Name);
        }

        private void Intercept(IInvocation invocation, TransactionalAttribute attribute)
        {
            if (invocation.Method.IsAsync())
                InterceptAsync(invocation, attribute);
            else
                InterceptSync(invocation, attribute);
        }

        private static TransactionalAttribute FindTransactionalAttribute(MemberInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<TransactionalAttribute>(true) ??
                   methodInfo.DeclaringType?.GetCustomAttribute<TransactionalAttribute>(true);
        }

        private void InterceptAsync(IInvocation invocation, TransactionalAttribute attribute)
        {
            _logger.LogInformation("BeginTransaction with IsolationLevel: {IsolationLevel}", attribute.IsolationLevel);

            _dbContext.BeginTransaction(attribute.IsolationLevel);

            try
            {
                invocation.Proceed();
            }
            catch (Exception)
            {
                _dbContext.RollbackTransaction();
                throw;
            }

            if (invocation.Method.ReturnType == typeof(Task))
                invocation.ReturnValue = InterceptAsync((Task) invocation.ReturnValue, _dbContext);
            else //Task<TResult>
                invocation.ReturnValue = typeof(TransactionInterceptor)
                    .GetMethod(nameof(InterceptWithResultAsync),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0])
                    .Invoke(null, new[] {invocation.ReturnValue, _dbContext});
        }

        private static async Task InterceptAsync(Task task, IDbContext dbContext)
        {
            try
            {
                await task.ConfigureAwait(false);
                await dbContext.CommitTransactionAsync();
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }
        }

        private static async Task<T> InterceptWithResultAsync<T>(Task<T> task, IDbContext dbContext)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                if (result is Result {Failed: true})
                {
                    dbContext.RollbackTransaction();
                }
                else
                {
                    await dbContext.CommitTransactionAsync();
                }

                return result;
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }
        }

        private void InterceptSync(IInvocation invocation, TransactionalAttribute attribute)
        {
            _logger.LogInformation("BeginTransaction with IsolationLevel: {IsolationLevel}", attribute.IsolationLevel);

            _dbContext.BeginTransaction(attribute.IsolationLevel);
            try
            {
                invocation.Proceed();

                if (invocation.ReturnValue is Result {Failed: true})
                {
                    _dbContext.RollbackTransaction();
                }
                else
                {
                    _dbContext.CommitTransaction();
                }
            }
            catch (Exception)
            {
                _dbContext.RollbackTransaction();
                throw;
            }
        }
    }
}