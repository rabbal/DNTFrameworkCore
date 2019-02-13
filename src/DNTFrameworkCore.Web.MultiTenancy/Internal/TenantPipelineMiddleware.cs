using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.MultiTenancy.Internal
{
  public class TenantPipelineMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly Action<TenantPipelineBuilderContext, IApplicationBuilder> _configuration;

        private readonly ConcurrentDictionary<TenantInfo, Lazy<RequestDelegate>> _pipelines
            = new ConcurrentDictionary<TenantInfo, Lazy<RequestDelegate>>();

        public TenantPipelineMiddleware(
            RequestDelegate next, 
            IApplicationBuilder rootApp, 
            Action<TenantPipelineBuilderContext, IApplicationBuilder> configuration)
        {
            Guard.ArgumentNotNull(next, nameof(next));
            Guard.ArgumentNotNull(rootApp, nameof(rootApp));
            Guard.ArgumentNotNull(configuration, nameof(configuration));

            _next = next;
            _rootApp = rootApp;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            Guard.ArgumentNotNull(context, nameof(context));

            var tenantContext = context.GetTenantContext();

            if (tenantContext != null)
            {
                var tenantPipeline = _pipelines.GetOrAdd(
                    tenantContext.Tenant, 
                    new Lazy<RequestDelegate>(() => BuildTenantPipeline(tenantContext)));

                await tenantPipeline.Value(context);
            }
        }

        private RequestDelegate BuildTenantPipeline(TenantContext tenantContext)
        {
            var branchBuilder = _rootApp.New();

            var builderContext = new TenantPipelineBuilderContext
            {
                TenantContext = tenantContext,
                Tenant = tenantContext.Tenant
            };

            _configuration(builderContext, branchBuilder);

            // register root pipeline at the end of the tenant branch
            branchBuilder.Run(_next);

            return branchBuilder.Build();
        }
    }
}