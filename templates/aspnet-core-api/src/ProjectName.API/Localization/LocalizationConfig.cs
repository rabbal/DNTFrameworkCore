using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ProjectName.Resources;
using ProjectName.Resources.Resources;

namespace ProjectName.API.Localization
{
    public static class LocalizationConfig
    {
        public static IMvcBuilder AddCustomLocalization(this IMvcBuilder mvcBuilder, IServiceCollection services)
        {
            mvcBuilder.AddDataAnnotationsLocalization(options =>
            {
                const string resourcesPath = "Resources";
                string baseName = $"{resourcesPath}.{nameof(SharedResource)}";
                var location = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName).Name;

                options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(baseName, location);
            });

            services.AddLocalization();
            services.AddScoped<IStringLocalizer>(provider =>
                provider.GetRequiredService<IStringLocalizer<SharedResource>>());

            services.AddTranslation();
            return mvcBuilder;
        }
        
        public static IApplicationBuilder UseCustomRequestLocalization(this IApplicationBuilder app)
        {
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("fa-IR")),
                SupportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fa-IR")
                },
                SupportedUICultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fa-IR")
                }
            };
            app.UseRequestLocalization(requestLocalizationOptions);
            return app;
        }
    }
}