using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.ReflectionToolkit;
using DNTFrameworkCore.Threading;
using DNTFrameworkCore.Transaction;
using NHibernate;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace DNTFrameworkCore.NHibernate.Transaction
{
    public class TransactionInterceptor : IInterceptor
    {
        private readonly ISession _session;

        public TransactionInterceptor(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
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
            if (!attribute.HasValue || _session.Transaction != null)
            {
                invocation.Proceed();
                return;
            }

            Intercept(invocation, attribute.Value);
        }

        private void Intercept(IInvocation invocation, TransactionalAttribute attribute)
        {
            if (invocation.Method.IsAsync())
                InterceptAsync(invocation, attribute);
            else
                InterceptSync(invocation, attribute);
        }

        private static Maybe<TransactionalAttribute> FindTransactionalAttribute(MemberInfo methodInfo)
        {
            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TransactionalAttribute>(
                methodInfo);
        }

        private void InterceptAsync(IInvocation invocation, TransactionalAttribute attribute)
        {
            _session.BeginTransaction(attribute.IsolationLevel);

            try
            {
                invocation.Proceed();
            }
            catch (Exception)
            {
                _session.Transaction.Rollback();
                throw;
            }

            if (invocation.Method.ReturnType == typeof(Task))
                invocation.ReturnValue = InterceptAsync((Task) invocation.ReturnValue, _session);
            else //Task<TResult>
                invocation.ReturnValue = typeof(TransactionInterceptor)
                    .GetMethod(nameof(InterceptWitResultAsync),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0])
                    .Invoke(null, new[] {invocation.ReturnValue, _session});
        }

        private static async Task InterceptAsync(Task task, ISession session)
        {
            try
            {
                await task.ConfigureAwait(false);
                session.Transaction.Commit();
            }
            catch (Exception)
            {
                session.Transaction.Rollback();
                throw;
            }
        }

        private static async Task<T> InterceptWitResultAsync<T>(Task<T> task, ISession session)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                if (result is Result returnValue && returnValue.Failed)
                    session.Transaction.Rollback();
                else
                    session.Transaction.Commit();

                return result;
            }
            catch (Exception)
            {
                session.Transaction.Rollback();
                throw;
            }
        }

        private void InterceptSync(IInvocation invocation, TransactionalAttribute attribute)
        {
            _session.BeginTransaction(attribute.IsolationLevel);
            try
            {
                invocation.Proceed();

                if (invocation.ReturnValue is Result returnValue && returnValue.Failed)
                    _session.Transaction.Rollback();
                else
                    _session.Transaction.Commit();
            }
            catch (Exception)
            {
                _session.Transaction.Rollback();
                throw;
            }
        }
    }
}