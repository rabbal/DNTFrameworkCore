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
        private readonly IHttpContextAccessor _context;
        private readonly IUserSession _session;

        public TenantSession(IHttpContextAccessor context, IUserSession session)
        {
            _session = Ensure.IsNotNull(session, nameof(session));
            _context = Ensure.IsNotNull(context, nameof(context));
        }

        private ClaimsPrincipal Principal => _context?.HttpContext?.User;
        private Tenant Tenant => _context?.HttpContext?.GetTenant();
        public string TenantId => _session.IsAuthenticated ? Principal?.FindTenantId() : Tenant?.Id;
        public string TenantName => _session.IsAuthenticated ? Principal?.FindTenantName() : Tenant?.Name;
        public bool IsHeadTenant => Principal?.IsHeadTenant() ?? false;
        public string ImpersonatorTenantId => Principal?.FindImpersonatorTenantId();
    }
}