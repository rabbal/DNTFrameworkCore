using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ProjectName.Common.Localization;
using ProjectName.Resources.Resources;

namespace ProjectName.Resources.Localization
{
    internal sealed class TranslationService : ITranslationService
    {
        private const string ResourceSymbol = "::";
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly ILogger<TranslationService> _logger;

        public TranslationService(IStringLocalizerFactory localizerFactory, ILogger<TranslationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
        }

        public string this[string index] => Translate(index);

        public string Translate(string key, params object[] arguments)
        {
            var result = LocalizerFactory(key).GetString(key, arguments);
            LogIfNotFound(key, result);
            return result;
        }

        public string Translate(string key)
        {
            var result = LocalizerFactory(key).GetString(key);
            LogIfNotFound(key, result);
            return result;
        }

        private void LogIfNotFound(string key, LocalizedString result)
        {
            if (!result.ResourceNotFound) return;
            var culture = CultureInfo.CurrentCulture.DisplayName;
            _logger.LogError(
                $"The localization resource with culture:`{culture}` & name:`{key}` not found. SearchedLocation: `{result.SearchedLocation}`.");
        }

        private IStringLocalizer LocalizerFactory(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            
            var resourceName = nameof(SharedResource);
            var resourceSymbolIndex = key.IndexOf(ResourceSymbol, StringComparison.Ordinal);
            if (resourceSymbolIndex >= 0)
                resourceName = key.Substring(0, resourceSymbolIndex);

            const string resourcesPath = "Resources";
            var baseName = $"{resourcesPath}.{resourceName}";
            var location = new AssemblyName(GetType().GetTypeInfo().Assembly.FullName).Name;

            return _localizerFactory.Create(baseName, location);
        }
    }
}