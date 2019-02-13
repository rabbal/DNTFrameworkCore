using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Configuration.ClientVisibilityProviders
{
    public class RequiresPermissionSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        private readonly IPermissionDependency _permissionDependency;

        public RequiresPermissionSettingClientVisibilityProvider(IPermissionDependency permissionDependency)
        {
            _permissionDependency = permissionDependency;
        }

        public Task<bool> CheckVisibleAsync(IServiceProvider serviceProvider)
        {
            var session = serviceProvider.GetService<IUserSession>();

            if (!session.UserId.HasValue)
            {
                return Task.FromResult(false);
            }

            var permissionDependencyContext = serviceProvider.GetService<PermissionDependencyContext>();

            return Task.FromResult(_permissionDependency.IsSatisfied(permissionDependencyContext));
        }
    }
}