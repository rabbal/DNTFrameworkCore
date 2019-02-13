using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Application.Navigation
{
    internal class NavigationManager : INavigationManager
    {
        private readonly IServiceProvider _provider;
        private readonly IDictionary<string, MenuDefinition> _menus;
        public IReadOnlyDictionary<string, MenuDefinition> Menus => _menus.ToImmutableDictionary();
        public MenuDefinition MainMenu => Menus["MainMenu"];

        public NavigationManager(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            _menus = new Dictionary<string, MenuDefinition>
            {
                {"MainMenu", new MenuDefinition("MainMenu", new FixedLocalizableString("MainMenu"))}
            };

            Initialize();
        }

        private void Initialize()
        {
            var context = new NavigationProviderContext(this);

            using (var scope = _provider.CreateScope())
            {
                var providers = scope.ServiceProvider.GetServices<INavigationProvider>();
                foreach (var provider in providers)
                {
                    var menus = provider.ProvideNavigation(context);

                    foreach (var menu in menus)
                    {
                        if (_menus.ContainsKey(menu.Name))
                        {
                            throw new FrameworkException("There is already a menu with name: " + menu.Name);
                        }

                        _menus[menu.Name] = menu;
                    }
                }
            }
        }
    }
}