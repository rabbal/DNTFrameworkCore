using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Extensions;
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

        public IReadOnlyList<string> Permissions => _context?.HttpContext?.User?.Identity.GetPermissions();

        public IReadOnlyList<string> Roles => _context?.HttpContext?.User?.Identity.GetRoles();

        public string UserDisplayName => _context?.HttpContext?.User?.Identity.GetUserDisplayName();

        public string UserBrowserName => _context.HttpContext?.GetUserAgent();

        public string UserIP => _context.HttpContext?.GetIp();

        public long? UserId => _context?.HttpContext?.User?.Identity.GetUserId();

        public string UserName => _context?.HttpContext?.User?.Identity.Name;

        public long? TenantId => UserId.HasValue
            ? _context?.HttpContext?.User?.Identity.GetTenantId()
            : _tenant.Value?.Id;

        public MultiTenancySides MultiTenancySide => TenantId.HasValue
            ? MultiTenancySides.Tenant
            : MultiTenancySides.Host;

        public long? ImpersonatorUserId => _context?.HttpContext?.User?.Identity.GetImpersonatorTenantId();

        public long? ImpersonatorTenantId => _context?.HttpContext?.User?.Identity.GetImpersonatorUserId();

        public bool IsAuthenticated => _context?.HttpContext?.User?.Identity.IsAuthenticated ?? false;

        public IDisposable Use(long? tenantId, long? userId)
        {
            throw new NotImplementedException();
        }

        public bool IsInRole(string role)
        {
            return Roles.Any(roleName => roleName == role);
        }

        public bool IsGranted(string permission)
        {
            return Roles.Any(permissionName => permissionName == permission);
        }
    }
}