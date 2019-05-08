using System;
using DNTFrameworkCore.TestWebApp.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

            services.AddScoped<ICookieValidator, CookieValidator>();
            services.AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.SlidingExpiration = false;
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    //options.AccessDeniedPath = new PathString("/Home/Forbidden/");
                    options.Cookie.Name = "auth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.ExpireTimeSpan = TimeSpan.FromDays(15);
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = context =>
                        {
                            var validator = context
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<ICookieValidator>();

                            return validator.ValidateAsync(context);
                        }
                    };
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Allows injecting IUrlHelper as a dependency
            services.AddScoped(serviceProvider =>
            {
                var actionContext = serviceProvider.GetService<IActionContextAccessor>().ActionContext;
                var urlHelperFactory = serviceProvider.GetService<IUrlHelperFactory>();
                return urlHelperFactory?.GetUrlHelper(actionContext);
            });
            services.AddMvc()
                .AddMvcLocalization()
                .AddViewLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
    }
}