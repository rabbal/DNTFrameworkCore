using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Infrastructure;
using DNTFrameworkCore.Web.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace DNTFrameworkCore.Web
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDNTFramework(this IApplicationBuilder app)
        {
            IoC.ApplicationServices = app.ApplicationServices;

            Bootstrapper.RunOnStartup(app.ApplicationServices);

            app.UseMiddleware<FrameworkMiddleware>();

            return app;
        }

        public static IApplicationBuilder UseAngularPushStateRouting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AngularPushStateRoutingMiddleware>();
        }
    }
}