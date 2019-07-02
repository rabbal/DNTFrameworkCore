using System.Security.Claims;

namespace DNTFrameworkCore.Runtime
{
    public static class DNTClaimTypes
    {
        public const string UserName = ClaimTypes.Name;
        public const string GivenName = ClaimTypes.GivenName;
        public const string Surname = ClaimTypes.Surname;
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string SerialNumber = ClaimTypes.SerialNumber;
        public const string DisplayName = "https://www.DNTFramework.com/identity/claims/displayName";
        public const string UserData = ClaimTypes.UserData;
        public const string Role = ClaimTypes.Role;
        public const string BranchId = "https://www.DNTFramework.com/identity/claims/branchId";
        public const string BranchNumber = "https://www.DNTFramework.com/identity/claims/branchNumber";
        public const string TenantId = "https://www.DNTFramework.com/identity/claims/tenantId";
        public const string Permission = "https://www.DNTFramework.com/identity/claims/permission";
        public const string ImpersonatorUserId = "https://www.DNTFramework.com/identity/claims/impersonatorUserId";

        public const string ImpersonatorTenantId =
            "https://www.DNTFramework.com/identity/claims/impersonatorTenantId";
    }
}