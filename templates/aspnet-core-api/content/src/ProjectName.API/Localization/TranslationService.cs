using System;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ProjectName.Application.Localization;
using ProjectName.Resources.Resources;

namespace ProjectName.API.Localization
{
    /// <summary>
    /// TODO: Provide a mechanism to support multiple resource file
    /// </summary>
    internal sealed class TranslationService : ITranslationService
    {
        private readonly IStringLocalizer _localizer;
        private readonly ILogger<TranslationService> _logger;

        public TranslationService(IStringLocalizer<SharedResource> localizer, ILogger<TranslationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public string this[string index] => Translate(index);

        public string Translate(string key, params object[] arguments)
        {
            var result = _localizer.GetString(key, arguments);
            LogIfNotFound(key, result);
            return result;
        }

        public string Translate(string key)
        {
            var result = _localizer.GetString(key);
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
    }
}