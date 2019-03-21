using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestWebApp.Application;
using DNTFrameworkCore.TestWebApp.Infrastructure;
using DNTFrameworkCore.TestWebApp.Resources;
using DNTFrameworkCore.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddDNTFramework()
                .AddDataAnnotationValidation()
                .AddModelValidation()
                .AddFluentModelValidation()
                .AddValidationOptions(options =>
                {
                    /*options.IgnoredTypes.Add(typeof());*/
                })
                .AddMemoryCache()
                .AddAuditingOptions(options =>
                {
                    // options.Enabled = true;
                    // options.EnabledForAnonymousUsers = false;
                    // options.IgnoredTypes.Add(typeof());
                    // options.Selectors.Add(new NamedTypeSelector("SelectorName", type => type == typeof()));
                }).AddTransactionOptions(options =>
                {
                    // options.Timeout=TimeSpan.FromMinutes(3);
                    //options.IsolationLevel=IsolationLevel.ReadCommitted;
                });
            services.AddDNTCommonWeb();
            services.AddInfrastructure(Configuration);
            services.AddApplication(Configuration);
            services.AddResources();
            services.AddWeb();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseDNTFramework();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}