using System;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Configuration.ClientVisibilityProviders
{
    public class HiddenSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisibleAsync(IServiceProvider scope)
        {
            return await Task.FromResult(false);
        }
    }
}