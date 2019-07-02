using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Extensions
{
    public static class IdentityExtensions
    {
        public static T FindUserId<T>(this IIdentity identity) where T : IConvertible
        {
            var id = identity?.FindUserClaimValue(DNTClaimTypes.UserId);

            if (id != null) return (T) Convert.ChangeType(id, typeof(T), CultureInfo.InvariantCulture);

            return default;
        }

        public static long? FindUserId(this IIdentity identity)
        {
            var id = identity?.FindUserClaimValue(DNTClaimTypes.UserId);

            if (id.IsEmpty()) return null;

            if (!long.TryParse(id, out var userId)) return null;

            return userId;
        }
        
        public static string FindBranchNumber(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(DNTClaimTypes.BranchNumber);
        }

        public static long? FindBranchId(this IIdentity identity)
        {
            var branchClaim = identity?.FindUserClaimValue(DNTClaimTypes.BranchId);

            if (branchClaim.IsEmpty()) return default;

            return !long.TryParse(branchClaim, out var branchId) ? default : branchId;
        }
        public static long? FindTenantId(this IIdentity identity)
        {
            var tenantClaim = identity?.FindUserClaimValue(DNTClaimTypes.TenantId);

            if (tenantClaim.IsEmpty()) return default;

            return !long.TryParse(tenantClaim, out var tenantId) ? default : tenantId;
        }

        public static long? FindImpersonatorTenantId(this IIdentity identity)
        {
            var tenantClaim = identity?.FindUserClaimValue(DNTClaimTypes.ImpersonatorTenantId);

            if (tenantClaim.IsEmpty()) return default;

            return !long.TryParse(tenantClaim, out var tenantId) ? default : tenantId;
        }

        public static long? FindImpersonatorUserId(this IIdentity identity)
        {
            var tenantClaim = identity?.FindUserClaimValue(DNTClaimTypes.ImpersonatorUserId);

            if (tenantClaim.IsEmpty()) return default;

            return !long.TryParse(tenantClaim, out var tenantId) ? default : tenantId;
        }

        public static IReadOnlyList<string> FindPermissions(this IIdentity identity)
        {
            Guard.ArgumentNotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var permissionClaims = claimsIdentity?.FindAll(DNTClaimTypes.Permission);

            if (permissionClaims == null) return new List<string>();

            return permissionClaims.Select(a => a.Value).ToImmutableList();
        }

        public static IReadOnlyList<string> FindRoles(this IIdentity identity)
        {
            Guard.ArgumentNotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var roleClaims = claimsIdentity?.FindAll(DNTClaimTypes.Role);

            if (roleClaims == null) return new List<string>();

            return roleClaims.Select(a => a.Value).ToImmutableList();
        }

        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity?.FindFirst(claimType)?.Value;
        }

        public static string FindUserClaimValue(this IIdentity identity, string claimType)
        {
            return (identity as ClaimsIdentity)?.FindFirstValue(claimType);
        }

        public static string FindUserFirstName(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(DNTClaimTypes.GivenName);
        }

        public static string FindUserLastName(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(DNTClaimTypes.Surname);
        }

        public static string FindUserFullName(this IIdentity identity)
        {
            return $"{FindUserFirstName(identity)} {FindUserLastName(identity)}";
        }

        public static string FindUserDisplayName(this IIdentity identity)
        {
            var fullName = FindUserFullName(identity);
            return string.IsNullOrWhiteSpace(fullName) ? FindUserName(identity) : fullName;
        }

        public static string FindUserName(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(DNTClaimTypes.UserName);
        }
    }
}