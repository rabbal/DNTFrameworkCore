using System.Collections.Generic;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Application.Navigation
{
    public interface INavigationManager : ISingletonDependency
    {
        /// <summary>
        /// All menus defined in the application.
        /// </summary>
        IReadOnlyDictionary<string, MenuDefinition> Menus { get; }

        /// <summary>
        /// Gets the main menu of the application.
        /// A shortcut of <see cref="Menus"/>["MainMenu"].
        /// </summary>
        MenuDefinition MainMenu { get; }
    }
}