using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Functional;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Configuration
{
    /// <summary>
    /// Defines setting definition manager.
    /// </summary>
    public interface ISettingDefinitionService : ISingletonDependency
    {
        /// <summary>
        /// Gets the <see cref="SettingDefinition"/> object with given unique name.
        /// Throws exception if can not find the setting.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>The <see cref="SettingDefinition"/> object.</returns>
        Maybe<SettingDefinition> Find(string name);

        /// <summary>
        /// Gets a list of all setting definitions.
        /// </summary>
        /// <returns>All settings.</returns>
        IReadOnlyList<SettingDefinition> ReadList();
    }

    internal class SettingDefinitionService : ISettingDefinitionService
    {
        private readonly IServiceProvider _provider;
        private readonly IDictionary<string, SettingDefinition> _settings =
            new Dictionary<string, SettingDefinition>();

        public SettingDefinitionService(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            Initialize();
        }

        private void Initialize()
        {
            using (var scope = _provider.CreateScope())
            {
                var providers = scope.ServiceProvider.GetServices<ISettingProvider>();
                foreach (var provider in providers)
                {
                    var settings = provider.ProvideSettings().ToList();
                    foreach (var setting in settings)
                    {
                        _settings[setting.Name] = setting;
                    }
                }
            }
        }

        public Maybe<SettingDefinition> Find(string name)
        {
            return !_settings.TryGetValue(name, out var settingDefinition)
                ? Maybe<SettingDefinition>.None
                : settingDefinition;
        }

        public IReadOnlyList<SettingDefinition> ReadList()
        {
            return _settings.Values.ToImmutableList();
        }
    }
}