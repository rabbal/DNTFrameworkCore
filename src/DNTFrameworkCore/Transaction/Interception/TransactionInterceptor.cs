using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.ReflectionToolkit;
using DNTFrameworkCore.Threading;

namespace DNTFrameworkCore.Transaction.Interception
{
    public class TransactionInterceptor : IInterceptor
    {
        private readonly ITransactionProvider _provider;

        public TransactionInterceptor(ITransactionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
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
            if (!attribute.HasValue || _provider.CurrentTransaction != null)
            {
                invocation.Proceed();
                return;
            }

            Intercept(invocation, attribute.Value);
        }

        private void Intercept(IInvocation invocation, TransactionalAttribute attribute)
        {
            if (invocation.Method.IsAsync())
            {
                InterceptAsync(invocation, attribute);
            }
            else
            {
                InterceptSync(invocation, attribute);
            }
        }

        private static Maybe<TransactionalAttribute> FindTransactionalAttribute(MemberInfo methodInfo)
        {
            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TransactionalAttribute>(
                methodInfo);
        }

        private void InterceptAsync(IInvocation invocation, TransactionalAttribute attribute)
        {
            var transaction = _provider.BeginTransaction(attribute.IsolationLevel);

            try
            {
                invocation.Proceed();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InterceptAsync((Task) invocation.ReturnValue, transaction);
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = typeof(TransactionInterceptor)
                    .GetMethod(nameof(InterceptWitResultAsync),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0])
                    .Invoke(null, new[] {invocation.ReturnValue, transaction});
            }
        }

        private static async Task InterceptAsync(Task task, ITransaction transaction)
        {
            try
            {
                await task.ConfigureAwait(false);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private static async Task<T> InterceptWitResultAsync<T>(Task<T> task, ITransaction transaction)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                if (result is Result returnValue && returnValue.Failed)
                {
                    transaction.Rollback();
                }
                else
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private void InterceptSync(IInvocation invocation, TransactionalAttribute attribute)
        {
            var transaction = _provider.BeginTransaction(attribute.IsolationLevel);
            try
            {
                invocation.Proceed();

                if (invocation.ReturnValue is Result returnValue && returnValue.Failed)
                {
                    transaction.Rollback();
                }
                else
                {
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}