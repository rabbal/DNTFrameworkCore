using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Extensions
{
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Returns the value for the first claim of the specified type otherwise null the claim is not present.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
        /// <param name="claimType">The claim type whose first value should be returned.</param>
        /// <returns>The value of the first instance of the specified claim type, or null if the claim is not present.</returns>        
        public static string FindFirstValue(this ClaimsPrincipal principal, string claimType)
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
                .SelectMany(c => c.Value.UnpackFromString(PermissionConstant.PackingSymbol));

            permissions.AddRange(packedPermissions);

            return permissions.AsReadOnly();
        }

        public static bool HasPermission(this ClaimsPrincipal principal, string permission)
        {
            return principal.FindPermissions().Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase));
        }

        public static string FindUserId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(UserClaimTypes.UserId);
            return value;
        }

        public static string FindBranchName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.BranchName);
        }

        public static string FindBranchId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.BranchId);
        }

        public static string FindTenantId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.TenantId);
        }

        public static string FindTenantName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.TenantName);
        }

        public static bool IsHeadTenant(this ClaimsPrincipal principal)
        {
            return principal.Claims.Any(c =>
                c.Type == UserClaimTypes.IsHeadTenant && c.Value == "true");
        }
        
        public static bool IsHeadOffice(this ClaimsPrincipal principal)
        {
            return principal.Claims.Any(c =>
                c.Type == UserClaimTypes.IsHeadOffice && c.Value == "true");
        }

        public static string FindImpersonatorTenantId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.ImpersonatorTenantId);
        }

        public static string FindImpersonatorUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.ImpersonatorUserId);
        }

        public static string FindUserDisplayName(this ClaimsPrincipal principal)
        {
            var displayName = principal.FindFirstValue(UserClaimTypes.UserName);
            return string.IsNullOrWhiteSpace(displayName) ? FindUserName(principal) : displayName;
        }

        public static string FindUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.UserName);
        }
    }
}