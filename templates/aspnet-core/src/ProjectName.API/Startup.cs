using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProjectName.API.Hubs;
using ProjectName.Application;
using ProjectName.Application.Configuration;
using ProjectName.Infrastructure;
using ProjectName.Resources;

namespace ProjectName.API
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
            services.Configure<ProjectOptions>(Configuration.Bind);
            
            services.AddFramework();
            services.AddInfrastructure(Configuration);
            services.AddApplication();
            services.AddResources();
            services.AddWebApp();
            services.AddJwtAuthentication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Use(async (context, next) =>
                    {
                        var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                        if (error?.Error is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                Message = "authentication token expired"
                            }));
                        }
                        else if (error?.Error != null)
                        {
                            context.Response.StatusCode = 500;
                            context.Response.ContentType = "application/json";
                            const string message = "متأسفانه مشکلی در فرآیند انجام درخواست شما پیش آمده است!";

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                Message = message
                            }));
                        }
                        else
                        {
                            await next();
                        }
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSignalR(routes => { routes.MapHub<NotificationHub>("/api/notificationhub"); });
        }
    }
}