using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.Localization
{
    public static class LocalizationExtensions
    {
        public static void AddNullLocalization(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizerFactory, NullStringLocalizerFactory>();
        }

        public static bool IsValidCultureName(this string cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                return false;
            }

            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }
}