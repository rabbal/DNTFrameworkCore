using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Configuration.ClientVisibilityProviders
{
    public class RequiresAuthenticationSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisibleAsync(IServiceProvider serviceProvider)
        {
            return await Task.FromResult(
                serviceProvider.GetService<IUserSession>().IsAuthenticated
            );
        }
    }
}