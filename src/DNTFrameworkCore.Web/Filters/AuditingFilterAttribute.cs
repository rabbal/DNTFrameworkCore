using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DNTFrameworkCore.Auditing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Filters
{
    public class AuditingFilterAttribute : IAsyncActionFilter
    {
        private readonly IOptions<AuditingOptions> _configuration;
        private readonly IAuditingHelper _helper;

        public AuditingFilterAttribute(IOptions<AuditingOptions> configuration, IAuditingHelper helper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldAudit(context))
            {
                await next();
                return;
            }

            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var auditInfo = _helper.BuildAuditInfo(
                    descriptor.ControllerTypeInfo,
                    descriptor.MethodInfo,
                    context.ActionArguments
                );

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    var result = await next();

                    if (result.Exception != null && !result.ExceptionHandled)
                    {
                        auditInfo.Exception = result.Exception;
                    }
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
                    _helper.Save(auditInfo);
                }
            }
        }

        private bool ShouldAudit(ActionContext actionContext)
        {
            return _configuration.Value.Enabled &&
                   actionContext.ActionDescriptor is ControllerActionDescriptor descriptor &&
                   _helper.ShouldAudit(descriptor.MethodInfo, true);
        }
    }
}