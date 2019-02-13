using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Threading;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DNTFrameworkCore.Auditing
{
    public class AuditingInterceptor : IInterceptor
    {
        private readonly IAuditingHelper _helper;
        private readonly IOptions<AuditingOptions> _options;

        public AuditingInterceptor(IAuditingHelper helper, IOptions<AuditingOptions> options)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _options = options ?? throw new ArgumentNullException(nameof(helper));
        }

        public void Intercept(IInvocation invocation)
        {
            if (!_helper.ShouldAudit(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = _helper.BuildAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget,
                invocation.Arguments);

            if (invocation.Method.IsAsync())
            {
                PerformAsyncAuditing(invocation, auditInfo);
            }
            else
            {
                PerformSyncAuditing(invocation, auditInfo);
            }
        }

        private void PerformSyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                if (_options.Value.SaveReturnValues && invocation.ReturnValue != null)
                    auditInfo.ReturnValue = JsonConvert.SerializeObject(invocation.ReturnValue);
                _helper.Save(auditInfo);
            }
        }

        private void PerformAsyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            invocation.Proceed();

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithFinally(
                    (Task) invocation.ReturnValue,
                    exception => SaveAuditInfo(auditInfo, stopwatch, exception, null)
                );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    (exception, task) => SaveAuditInfo(auditInfo, stopwatch, exception, task)
                );
            }
        }

        private void SaveAuditInfo(AuditInfo auditInfo, Stopwatch stopwatch, Exception exception, Task task)
        {
            stopwatch.Stop();
            auditInfo.Exception = exception;
            auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            FillTaskResult(task, auditInfo);

            _helper.Save(auditInfo);
        }

        private void FillTaskResult(Task task, AuditInfo auditInfo)
        {
            if (_options.Value.SaveReturnValues && task != null &&
                task.Status == TaskStatus.RanToCompletion)
            {
                auditInfo.ReturnValue = JsonConvert.SerializeObject(task.GetType().GetTypeInfo()
                    .GetProperty("Result")
                    ?.GetValue(task, null));
            }
        }
    }
}