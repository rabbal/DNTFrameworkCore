using System.Collections.Generic;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Configuration
{
    /// <summary>
    /// This is the main interface to define setting for an module/application.
    /// </summary>
    public interface ISettingProvider : ITransientDependency
    {
        IEnumerable<SettingDefinition> ProvideSettings();
    }
}