using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Extensions
{
    public static class PrincipalExtensions
    {
        public static Maybe<string> FindFirstValue(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            var claim = principal.FindFirst(claimType);
            return claim?.Value;
        }

        public static IReadOnlyList<string> FindRoles(this ClaimsPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            var roles = principal.Claims
                .Where(c => c.Type.Equals(UserClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value).ToList();

            return roles.AsReadOnly();
        }

        public static IReadOnlyList<string> FindPermissions(this ClaimsPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));
            
            var permissions = principal.Claims
                .Where(c => c.Type.Equals(UserClaimTypes.Permission, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToList();

            var packedPermissions = principal.Claims.Where(c =>
                    c.Type.Equals(UserClaimTypes.PackedPermission, StringComparison.OrdinalIgnoreCase))
                .SelectMany(c => c.Value.UnpackPermissionsFromString());
            
            permissions.AddRange(packedPermissions);
            
            return permissions.AsReadOnly();
        }

        public static bool HasPermission(this ClaimsPrincipal principal, string permission)
        {
            return principal.FindPermissions().Any(p => p.Equals(permission, StringComparison.Ordinal));
        }
    }
}