using System;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.IO;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Web.Authorization;
using DNTFrameworkCore.Web.Cryptography;
using DNTFrameworkCore.Web.Hosting;
using DNTFrameworkCore.Web.IO;
using DNTFrameworkCore.Web.Runtime;
using DNTFrameworkCore.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web
{
    public static class DependencyInjection
    {
        public static WebFrameworkBuilder AddWebFramework(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpContextAccessor();
            services.AddScoped<IUserSession, UserSession>();
            
            return new WebFrameworkBuilder(services);
        }
    }

    /// <summary>
    /// Configure DNTFrameworkCore.Web services
    /// </summary>
    public class WebFrameworkBuilder
    {
        public IServiceCollection Services { get; }

        public WebFrameworkBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public WebFrameworkBuilder WithProtection()
        {
            Services.AddSingleton<IProtectionService, ProtectionService>();
            return this;
        }

        public WebFrameworkBuilder WithPermissionAuthorization()
        {
            Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            return this;
        }

        public WebFrameworkBuilder WithAntiXsrf()
        {
            Services.AddScoped<IAntiXsrf, AntiXsrf>();
            return this;
        }

        public WebFrameworkBuilder WithPasswordHashAlgorithm()
        {
            Services.AddSingleton<IUserPasswordHashAlgorithm, UserPasswordHashAlgorithm>();
            return this;
        }

        public WebFrameworkBuilder WithQueuedHostedService()
        {
            Services.AddHostedService<QueuedHostedService>();
            return this;
        }
        
        public WebFrameworkBuilder WithTaskHostedService()
        {
            Services.AddHostedService<TaskHostedService>();
            return this;
        }
        
        public WebFrameworkBuilder WithEnvironmentPath()
        {
            Services.AddSingleton<IEnvironmentPath, EnvironmentPath>();
            return this;
        }

        /// <summary>
        /// Adds IFileNameSanitizer to IServiceCollection.
        /// </summary>
        public WebFrameworkBuilder WithSafeFileSanitizer()
        {
            Services.AddTransient<IFileNameSanitizer, FileNameSanitizer>();
            return this;
        }
    }
}