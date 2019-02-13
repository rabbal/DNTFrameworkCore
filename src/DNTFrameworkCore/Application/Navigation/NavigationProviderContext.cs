namespace DNTFrameworkCore.Application.Navigation
{
    public class NavigationProviderContext
    {
        public INavigationManager Manager { get; }

        public NavigationProviderContext(INavigationManager manager)
        {
            Manager = manager;
        }
    }
}