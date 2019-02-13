using System.Collections.Generic;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Application.Navigation
{
    /// <summary>
    /// This interface should be implemented by classes which change
    /// navigation of the application.
    /// </summary>
    public interface INavigationProvider : ITransientDependency
    {
        /// <summary>
        /// Used to set navigation.
        /// </summary>
        /// <param name="context">Navigation context</param>
       IEnumerable<MenuDefinition> ProvideNavigation(NavigationProviderContext context);
    }
}