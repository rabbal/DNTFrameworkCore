using System;
using System.Security.Claims;
using System.Security.Principal;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Extensions
{
    public static class IdentityExtensions
    {
        public static T FindUserId<T>(this IIdentity identity) where T : IEquatable<T>
        {
            return identity.FindUserId().FromString<T>();
        }

        public static T FindTenantId<T>(this IIdentity identity) where T : IEquatable<T>
        {
            return identity.FindTenantId().FromString<T>();
        }

        public static T FindBranchId<T>(this IIdentity identity) where T : IEquatable<T>
        {
            return identity.FindBranchId().FromString<T>();
        }

        public static string FindUserId(this IIdentity identity)
        {
            var value = identity.FindUserClaimValue(UserClaimTypes.UserId);
            return value;
        }

        public static string FindBranchName(this IIdentity identity)
        {
            return identity.FindUserClaimValue(UserClaimTypes.BranchName);
        }

        public static string FindBranchId(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(UserClaimTypes.BranchId);
        }

        public static string FindTenantId(this IIdentity identity)
        {
            return identity.FindUserClaimValue(UserClaimTypes.TenantId);
        }
        
        public static string FindTenantName(this IIdentity identity)
        {
            return identity.FindUserClaimValue(UserClaimTypes.TenantName);
        }

        public static string FindImpersonatorTenantId(this IIdentity identity)
        {
            return identity.FindUserClaimValue(UserClaimTypes.ImpersonatorTenantId);
        }

        public static string FindImpersonatorUserId(this IIdentity identity)
        {
            return identity.FindUserClaimValue(UserClaimTypes.ImpersonatorUserId);
        }

        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity.FindFirst(claimType)?.Value;
        }

        public static string FindUserClaimValue(this IIdentity identity, string claimType)
        {
            return (identity as ClaimsIdentity)?.FindFirstValue(claimType);
        }

        public static string FindUserDisplayName(this IIdentity identity)
        {
            var displayName = identity.FindUserClaimValue(UserClaimTypes.UserName);
            return string.IsNullOrWhiteSpace(displayName) ? FindUserName(identity) : displayName;
        }

        public static string FindUserName(this IIdentity identity)
        {
            return identity.FindUserClaimValue(UserClaimTypes.UserName);
        }
    }
}