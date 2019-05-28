using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Helpers;
using DNTFrameworkCore.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Authorization
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly LazyConcurrentDictionary<string, AuthorizationPolicy> _policies =
            new LazyConcurrentDictionary<string, AuthorizationPolicy>(StringComparer.OrdinalIgnoreCase);

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(PermissionAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return await base.GetPolicyAsync(policyName);
            }

            var policy = _policies.GetOrAdd(policyName, name =>
            {
                var permissionNames = policyName.Substring(PermissionAuthorizeAttribute.PolicyPrefix.Length).Split(',');

                return new AuthorizationPolicyBuilder()
                    .RequireClaim(DNTClaimTypes.Permission, permissionNames)
                    .Build();
            });

            return policy;
        }
    }
}