using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Extensions
{
    public static class IdentityExtensions
    {
        public static T FindUserId<T>(this IIdentity identity) where T : IConvertible
        {
            var id = identity?.FindUserClaimValue(UserClaimTypes.UserId);

            if (id != null) return (T) Convert.ChangeType(id, typeof(T), CultureInfo.InvariantCulture);

            return default;
        }

        public static long? FindUserId(this IIdentity identity)
        {
            var id = identity?.FindUserClaimValue(UserClaimTypes.UserId);

            if (id.IsEmpty()) return null;

            if (!long.TryParse(id, out var userId)) return null;

            return userId;
        }

        public static string FindBranchNumber(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(UserClaimTypes.BranchNumber);
        }

        public static long? FindBranchId(this IIdentity identity)
        {
            var branchClaim = identity?.FindUserClaimValue(UserClaimTypes.BranchId);

            if (branchClaim.IsEmpty()) return default;

            return !long.TryParse(branchClaim, out var branchId) ? default : branchId;
        }

        public static long? FindTenantId(this IIdentity identity)
        {
            var tenantClaim = identity?.FindUserClaimValue(UserClaimTypes.TenantId);

            if (tenantClaim.IsEmpty()) return default;

            return !long.TryParse(tenantClaim, out var tenantId) ? default : tenantId;
        }

        public static long? FindImpersonatorTenantId(this IIdentity identity)
        {
            var tenantClaim = identity?.FindUserClaimValue(UserClaimTypes.ImpersonatorTenantId);

            if (tenantClaim.IsEmpty()) return default;

            return !long.TryParse(tenantClaim, out var tenantId) ? default : tenantId;
        }

        public static long? FindImpersonatorUserId(this IIdentity identity)
        {
            var tenantClaim = identity?.FindUserClaimValue(UserClaimTypes.ImpersonatorUserId);

            if (tenantClaim.IsEmpty()) return default;

            return !long.TryParse(tenantClaim, out var tenantId) ? default : tenantId;
        }
        
        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity?.FindFirst(claimType)?.Value;
        }

        public static string FindUserClaimValue(this IIdentity identity, string claimType)
        {
            return (identity as ClaimsIdentity)?.FindFirstValue(claimType);
        }

        public static string FindUserDisplayName(this IIdentity identity)
        {
            var displayName = identity?.FindUserClaimValue(UserClaimTypes.UserName);
            return string.IsNullOrWhiteSpace(displayName) ? FindUserName(identity) : displayName;
        }

        public static string FindUserName(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(UserClaimTypes.UserName);
        }
    }
}