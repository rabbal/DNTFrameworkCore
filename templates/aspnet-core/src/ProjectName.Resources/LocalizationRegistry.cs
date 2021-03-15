using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ProjectName.Resources.Resources;

namespace ProjectName.Resources
{
    public static class LocalizationRegistry
    {
        public static void AddCustomLocalization(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizer>(provider =>
                provider.GetRequiredService<IStringLocalizer<SharedResource>>());
            services.AddSingleton<IStringLocalizer>(provider =>
                provider.GetRequiredService<IStringLocalizer<MessagesResource>>());
        }
    }
}