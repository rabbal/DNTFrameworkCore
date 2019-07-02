using System;
using System.Collections.Generic;
using System.Security.Claims;
using DNTFrameworkCore.Helpers;
using DNTFrameworkCore.MultiTenancy;

namespace DNTFrameworkCore.Runtime
{
    public class NullUserSession : IUserSession
    {
        public static readonly NullUserSession Instance = new NullUserSession();
        public bool IsAuthenticated => false;
        public long? UserId => null;
        public string UserName => string.Empty;
        public string BranchNumber => string.Empty;
        public IReadOnlyList<string> Permissions => new List<string>();
        public IReadOnlyList<string> Roles => new List<string>();
        public IReadOnlyList<Claim> Claims => new List<Claim>();
        public string UserDisplayName => string.Empty;
        public string UserBrowserName => string.Empty;
        public string UserIP => string.Empty;
        public long? TenantId => null;
        public MultiTenancySides MultiTenancySide => MultiTenancySides.Host;
        public long? ImpersonatorUserId => null;
        public long? ImpersonatorTenantId => null;
        public long? BranchId => null;

        public IDisposable Use(long? tenantId, long? userId)
        {
            return new DisposeAction(() => { });
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public bool IsGranted(string permission)
        {
            return false;
        }
    }
}