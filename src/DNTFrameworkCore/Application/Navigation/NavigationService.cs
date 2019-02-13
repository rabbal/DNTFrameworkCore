using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Features;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.Application.Navigation
{
    internal class NavigationService : INavigationService, IScopedDependency
    {
        private readonly INavigationManager _manager;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserSession _session;

        public NavigationService(
            IServiceProvider serviceProvider,
            IUserSession session,
            INavigationManager manager,
            IStringLocalizerFactory localizerFactory)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<Maybe<UserMenu>> FindAsync(string menuName)
        {
            var menuDefinition = _manager.Menus.ContainsKey(menuName) ? _manager.Menus[menuName] : null;
            if (menuDefinition == null)
            {
                return Maybe<UserMenu>.None;
            }

            var userMenu = new UserMenu(menuDefinition, _localizerFactory);
            var user = _session.ToUserIdentifier();
            await FillUserMenuItemsAsync(user, menuDefinition.Items, userMenu.Items);
            return userMenu;
        }

        public async Task<IReadOnlyList<UserMenu>> ReadListAsync()
        {
            var userMenus = new List<UserMenu>();

            foreach (var menu in _manager.Menus.Values)
            {
                var menuItem = await FindAsync(menu.Name);
                if (menuItem.HasValue)
                {
                    userMenus.Add(menuItem.Value);
                }
            }

            return userMenus;
        }

        private async Task<int> FillUserMenuItemsAsync(IUserIdentifier user,
            IEnumerable<MenuItemDefinition> menuItemDefinitions,
            ICollection<UserMenuItem> userMenuItems)
        {
            var addedMenuItemCount = 0;

            using (var scope = _serviceProvider.CreateScope())
            {
                var permissionDependencyContext = new PermissionDependencyContext(scope.ServiceProvider);

                var featureDependencyContext = scope.ServiceProvider.GetService<FeatureDependencyContext>();

                foreach (var menuItemDefinition in menuItemDefinitions)
                {
                    if (menuItemDefinition.RequiresAuthentication && user == null)
                    {
                        continue;
                    }

                    if (menuItemDefinition.PermissionDependency != null &&
                        (user == null ||
                         !menuItemDefinition.PermissionDependency.IsSatisfied(permissionDependencyContext))
                    )
                    {
                        continue;
                    }

                    if (menuItemDefinition.FeatureDependency != null &&
                        (_session.MultiTenancySide == MultiTenancySides.Tenant ||
                         user?.TenantId != null) &&
                        !await menuItemDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext))
                    {
                        continue;
                    }

                    var userMenuItem = new UserMenuItem(menuItemDefinition, _localizerFactory);
                    if (menuItemDefinition.IsLeaf ||
                        await FillUserMenuItemsAsync(user, menuItemDefinition.Items, userMenuItem.Items) > 0)
                    {
                        userMenuItems.Add(userMenuItem);
                        ++addedMenuItemCount;
                    }
                }
            }

            return addedMenuItemCount;
        }
    }
}