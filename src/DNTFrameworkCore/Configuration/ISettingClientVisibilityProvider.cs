using System;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Configuration
{
    public interface ISettingClientVisibilityProvider
    {    
        Task<bool> CheckVisibleAsync(IServiceProvider serviceProvider);
    }
}