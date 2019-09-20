using System;
using System.Security.Claims;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Tenancy;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Tenancy.Internal
{
    internal sealed class TenantSession : ITenantSession
    {
        private readonly HttpContext _httpContext;
        private readonly ClaimsPrincipal _principal;
        private readonly IUserSession _session;

        public TenantSession(IHttpContextAccessor httpContext, IUserSession session)
        {
            _session = Ensure.IsNotNull(session, nameof(session));
            _httpContext = Ensure.IsNotNull(httpContext, nameof(httpContext)).HttpContext;
            _principal = _httpContext?.User;
        }

        public string TenantId => _session.IsAuthenticated
            ? _principal?.FindTenantId()
            : Tenant?.Id;

        public string TenantName => _session.IsAuthenticated
            ? _principal?.FindTenantName()
            : Tenant?.Name;

        public bool IsHeadTenant => _principal?.IsHeadTenant() ?? false;

        public string ImpersonatorTenantId => _principal?.FindImpersonatorTenantId();
        public Tenant Tenant => _httpContext?.GetTenant();

        public IDisposable UseTenantId(string tenantId)
        {
            throw new NotImplementedException();
        }
    }
}