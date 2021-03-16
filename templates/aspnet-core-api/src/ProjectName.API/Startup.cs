using DNTFrameworkCore;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.Web;
using DNTFrameworkCore.Web.ExceptionHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectName.API.Hubs;
using ProjectName.Application;
using ProjectName.Application.Configuration;
using ProjectName.Infrastructure;

namespace ProjectName.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ProjectOptions>(_configuration.Bind);
            services.Configure<ExceptionOptions>(_configuration.GetSection("Exception"));

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

            services.AddInfrastructure(_configuration);
            services.AddApplication();
            services.AddWebApp();
            services.AddJwtAuthentication(_configuration);

            // services.AddDistributedSqlServerCache(options =>
            // {
            //     options.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            //     options.SchemaName = "dbo";
            //     options.TableName = "Cache";
            // });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIf(env.IsProduction(), _ => _.UseHsts());

            app.UseExceptionHandling();
            // var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
            // if (error?.Error is SecurityTokenExpiredException)
            // {
            //     context.Response.StatusCode = 401;
            //     context.Response.ContentType = "application/json";
            //     await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            //     {
            //         Message = "authentication token expired"
            //     }));
            // }
            // else if (error?.Error != null)
            // {
            //     context.Response.StatusCode = 500;
            //     context.Response.ContentType = "application/json";
            //     const string message = "متأسفانه مشکلی در فرآیند انجام درخواست شما پیش آمده است!";
            //
            //     await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            //     {
            //         Message = message
            //     }));
            // }
            // else
            // {
            //     await next();
            // }
            
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectName API V1"); });

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/hubs/notification");
            });
        }
    }
}