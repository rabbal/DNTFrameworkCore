using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ProjectName.Resources.Resources;

namespace ProjectName.Resources
{
    public static class LocalizationRegistry
    {
        public static void AddTranslation(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizer>(provider =>
                provider.GetRequiredService<IStringLocalizer<SharedResource>>());
            //TODO: Provide mechanism to support multiple resource file
            // services.AddSingleton<IStringLocalizer>(provider =>
            //     provider.GetRequiredService<IStringLocalizer<MessagesResource>>());
        }
    }
}