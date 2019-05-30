using System.Security.Claims;
using System.Security.Principal;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Web.Authorization;
using DNTFrameworkCore.Web.Cryptography;
using DNTFrameworkCore.Web.Hosting;
using DNTFrameworkCore.Web.Runtime;
using DNTFrameworkCore.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDNTCommonWeb(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserSession, UserSession>();
            services.AddScoped<IPrincipal>(provider =>
                provider.GetService<IHttpContextAccessor>()?.HttpContext?.User ?? ClaimsPrincipal.Current);
            services.AddSingleton<IProtectionProvider, ProtectionProvider>();
            services.AddSingleton<IUserPassword, UserPassword>();
            services.AddScoped<IAntiforgeryService, AntiforgeryService>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddHostedService<QueuedHostedService>();

            return services;
        }

        public static IServiceCollection AddDNTDataProtection(this IServiceCollection services)
        {
            services.AddSingleton<IXmlRepository, XmlRepository>();

            services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(provider =>
            {
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    using (var scope = provider.CreateScope())
                    {
                        options.XmlRepository = scope.ServiceProvider.GetService<IXmlRepository>();
                    }
                });
            });

            return services;
        }
    }
}