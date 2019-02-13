using System;
using System.Threading.Tasks;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.MultiTenancy.Internal
{
    internal class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantResolutionMiddleware> _logger;

        public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
        {
            Guard.ArgumentNotNull(next, nameof(next));
            Guard.ArgumentNotNull(logger, nameof(logger));

            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ITenantResolver resolver, IConfiguration configuration,
            IServiceProvider provider)
        {
            Guard.ArgumentNotNull(context, nameof(context));
            Guard.ArgumentNotNull(resolver, nameof(resolver));
            Guard.ArgumentNotNull(configuration, nameof(configuration));

            _logger.LogDebug("Resolving Tenant using {loggerType}.", resolver.GetType().Name);

            var tenant = await resolver.ResolveAsync(context);

            if (tenant != null)
            {
                _logger.LogDebug("Tenant Resolved. Adding to HttpContext.");

                context.SetTenantContext(new TenantContext(tenant));
                
            }
            else
            {
                _logger.LogDebug("Tenant Not Resolved.");
            }

            await _next.Invoke(context);
        }
    }
}