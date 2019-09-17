using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Web.Http;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Runtime
{
    internal sealed class UserSession : IUserSession
    {
        private readonly HttpContext _httpContext;
        private readonly ClaimsPrincipal _principal;

        public UserSession(IHttpContextAccessor httpContext)
        {
            _httpContext = Ensure.IsNotNull(httpContext, nameof(httpContext)).HttpContext;
            _principal = _httpContext?.User;
        }

        public bool IsAuthenticated => _principal?.Identity.IsAuthenticated ?? false;
        public string UserId => _principal?.FindUserId();
        public string UserName => _principal?.FindUserName();
        public string BranchId => _principal?.FindBranchId();
        public string BranchName => _principal?.FindBranchName();
        public IReadOnlyList<string> Permissions => _principal?.FindPermissions();
        public IReadOnlyList<string> Roles => _principal?.FindRoles();
        public IReadOnlyList<Claim> Claims => _principal?.Claims.ToList();
        public string UserDisplayName => _principal?.FindUserDisplayName();
        public string UserBrowserName => _httpContext?.FindUserAgent();
        public string UserIP => _httpContext?.FindUserIP();
        public string ImpersonatorUserId => _principal?.FindImpersonatorUserId();

        public bool IsInRole(string role)
        {
            ThrowIfUnauthenticated();

            return _principal.IsInRole(role);
        }

        public bool IsGranted(string permission)
        {
            ThrowIfUnauthenticated();

            return _principal.HasPermission(permission);
        }

        public IDisposable UseUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public IDisposable UseBranchId(string branchId)
        {
            throw new NotImplementedException();
        }

        private void ThrowIfUnauthenticated()
        {
            if (!IsAuthenticated) throw new InvalidOperationException("This operation need user authenticated");
        }
    }
}