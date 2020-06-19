using System.Security.Claims;

namespace DNTFrameworkCore.Runtime
{
    public static class UserClaimTypes
    {
        public const string UserName = ClaimTypes.Name;
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string SecurityStamp = nameof(SecurityStamp);
        public const string Role = ClaimTypes.Role;
        public const string DisplayName = nameof(DisplayName);
        public const string BranchId = nameof(BranchId);
        public const string BranchName = nameof(BranchName);
        public const string IsHeadOffice = nameof(IsHeadOffice);
        public const string TenantId = nameof(TenantId);
        public const string TenantName = nameof(TenantName);
        public const string IsHeadTenant = nameof(IsHeadTenant);
        public const string Permission = nameof(Permission);
        public const string PackedPermission = nameof(PackedPermission);
        public const string ImpersonatorUserId = nameof(ImpersonatorUserId);
        public const string ImpersonatorTenantId = nameof(ImpersonatorTenantId);
    }
}