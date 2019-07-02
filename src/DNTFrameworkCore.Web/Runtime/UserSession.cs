using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Helpers;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Web.Http;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Runtime
{
    internal class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor _context;
        private readonly ITenant _tenant;

        public UserSession(
            IHttpContextAccessor context,
            ITenant tenant)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        public string BranchNumber => _context?.HttpContext?.User?.Identity.FindBranchNumber();
        public IReadOnlyList<string> Permissions => _context?.HttpContext?.User?.Identity.FindPermissions();

        public IReadOnlyList<string> Roles => _context?.HttpContext?.User?.Identity.FindRoles();
        public IReadOnlyList<Claim> Claims => _context?.HttpContext?.User?.Claims?.ToList();

        public string UserDisplayName => _context?.HttpContext?.User?.Identity.FindUserDisplayName();

        public string UserBrowserName => _context.HttpContext?.GetUserAgent();

        public string UserIP => _context.HttpContext?.GetIp();

        public long? UserId => _context?.HttpContext?.User?.Identity.FindUserId();

        public string UserName => _context?.HttpContext?.User?.Identity.Name;

        public long? BranchId => _context?.HttpContext?.User?.Identity.FindBranchId();

        public long? TenantId => UserId.HasValue
            ? _context?.HttpContext?.User?.Identity.FindTenantId()
            : _tenant.HasValue
                ? _tenant.Value.Id
                : (long?) null;

        public MultiTenancySides MultiTenancySide => TenantId.HasValue
            ? MultiTenancySides.Tenant
            : MultiTenancySides.Host;

        public long? ImpersonatorUserId => _context?.HttpContext?.User?.Identity.FindImpersonatorTenantId();

        public long? ImpersonatorTenantId => _context?.HttpContext?.User?.Identity.FindImpersonatorUserId();

        public bool IsAuthenticated => _context?.HttpContext?.User?.Identity.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return Roles.Any(roleName => roleName == role);
        }

        public bool IsGranted(string permission)
        {
            return Permissions.Any(permissionName => permissionName == permission);
        }
    }
}