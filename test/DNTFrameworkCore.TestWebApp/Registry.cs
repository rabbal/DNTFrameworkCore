using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestWebApp
{
    public static class Registry
    {
        public static void AddWeb(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization();
            services.AddHttpContextAccessor();
            services.AddMvc()
                .AddMvcLocalization()
                .AddViewLocalization()
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining<Program>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
    }
}