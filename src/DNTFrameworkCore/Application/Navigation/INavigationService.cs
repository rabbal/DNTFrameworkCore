using System.Collections.Generic;
using System.Threading.Tasks;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Application.Navigation
{
    /// <summary>
    /// Used to manage navigation for users.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets a menu specialized for given user.
        /// </summary>
        /// <param name="name">Unique name of the menu</param>
        Task<Maybe<UserMenu>> FindAsync(string name);

        /// <summary>
        /// Gets all menus specialized for given user.
        /// </summary>
        Task<IReadOnlyList<UserMenu>> ReadListAsync();
    }
}