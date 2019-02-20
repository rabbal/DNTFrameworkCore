using System;
using DNTFrameworkCore.EntityFramework;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.TestAPI.Application;
using DNTFrameworkCore.TestAPI.Hubs;
using DNTFrameworkCore.TestAPI.Infrastructure;
using DNTFrameworkCore.TestAPI.Infrastructure.Context;
using DNTFrameworkCore.TestAPI.Resources;
using DNTFrameworkCore.Web;
using EFSecondLevelCache.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestAPI
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
            
            services.AddDNTProtectionRepository<ProjectDbContext>();
            services.AddDNTCommonWeb()
                .AddDNTDataProtection();

            services.AddInfrastructure(Configuration);
            services.AddApplication(Configuration);
            services.AddResources();
            services.AddWeb();
            services.AddJwtAuthentication(Configuration);
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseDNTFramework();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSignalR(routes => { routes.MapHub<NotificationHub>("/api/notificationhub"); });
            app.UseEFSecondLevelCache();
        }
    }
}