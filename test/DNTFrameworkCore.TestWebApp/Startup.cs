using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestWebApp.Application;
using DNTFrameworkCore.TestWebApp.Infrastructure;
using DNTFrameworkCore.TestWebApp.Resources;
using DNTFrameworkCore.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DNTFrameworkCore.TestWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFramework()
                .WithModelValidation()
                .WithFluentValidation()
                .WithMemoryCache()
                .WithSecurityService()
                .WithBackgroundTaskQueue()
                .WithRandomNumber();

            services.AddWebFramework()
                .WithPermissionAuthorization()
                .WithProtection()
                .WithPasswordHashAlgorithm()
                .WithQueuedHostedService()
                .WithAntiXsrf()
                .WithEnvironmentPath();

            services.AddInfrastructure(Configuration);
            services.AddApplication(Configuration);
            services.AddResources();
            services.AddWeb();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // app.UseStatusCodePagesWithReExecute("/error/index/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseFileServer(new FileServerOptions
            {
                // Don't expose file system
                EnableDirectoryBrowsing = false
            });
            app.UseCookiePolicy();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}