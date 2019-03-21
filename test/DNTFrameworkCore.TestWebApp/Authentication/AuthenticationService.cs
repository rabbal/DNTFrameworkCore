using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.TestWebApp.Application.Identity;
using DNTFrameworkCore.TestWebApp.Resources;
using DNTFrameworkCore.Web.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.TestWebApp.Authentication
{
    public interface IAuthenticationService : IScopedDependency
    {
        Task<SignInResult> SignInAsync(string userName, string password);
        Task SignOutAsync();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;
        private readonly IMessageLocalizer _localizer;
        private readonly IHttpContextAccessor _context;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            IUserManager userManager,
            IRoleManager roleManager,
            IMessageLocalizer localizer,
            IHttpContextAccessor context,
            IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<SignInResult> SignInAsync(string userName, string password)
        {
            var userMaybe = await _userManager.FindByNameAsync(userName);
            if (!userMaybe.HasValue) return SignInResult.Failed(_localizer["SignIn.Messages.Failure"]);

            var user = userMaybe.Value;

            if (!_userManager.VerifyHashedPassword(user.PasswordHash, password))
                return SignInResult.Failed(_localizer["SignIn.Messages.Failure"]);

            if (!user.IsActive) return SignInResult.Failed(_localizer["SignIn.Messages.IsNotActive"]);

            var userId = user.Id;

            var claims = await GenerateCookieClaimsAsync(userId);

            var loginCookieExpirationDays = _configuration.GetValue<int>("LoginCookieExpirationDays", defaultValue: 30);
            await _context.HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               claims,
               new AuthenticationProperties
               {
                   IsPersistent = true, // "Remember Me"
                   IssuedUtc = DateTimeOffset.UtcNow,
                   ExpiresUtc = DateTimeOffset.UtcNow.AddDays(loginCookieExpirationDays)
               });

            await _userManager.UpdateLastActivityDateAsync(user);

            return SignInResult.Ok();
        }

        /// <summary>
        /// The Jwt implementation does not support "revoke OAuth token" (logout) by design.
        /// Delete the user's tokens from the database (revoke its bearer token)
        /// </summary>
        public async Task SignOutAsync()
        {
            await _context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private async Task<ClaimsPrincipal> GenerateCookieClaimsAsync(long userId)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);


            var userMaybe = await _userManager.FindIncludeClaimsAsync(userId);
            if (!userMaybe.HasValue) throw new InvalidOperationException("user not found!");

            var user = userMaybe.Value;

            identity.AddClaim(new Claim(DNTClaimTypes.UserId, user.Id.ToString(), ClaimValueTypes.Integer64));
            identity.AddClaim(new Claim(DNTClaimTypes.UserName, user.UserName, ClaimValueTypes.String));
            identity.AddClaim(new Claim(DNTClaimTypes.DisplayName, user.DisplayName, ClaimValueTypes.String));
            identity.AddClaim(new Claim(DNTClaimTypes.SerialNumber, user.SerialNumber, ClaimValueTypes.String));
            identity.AddClaim(new Claim(DNTClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String));

            foreach (var claim in user.Claims)
            {
                identity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue, ClaimValueTypes.String));
            }

            var roles = await _roleManager.FindUserRolesIncludeClaimsAsync(user.Id);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(DNTClaimTypes.Role, role.Name, ClaimValueTypes.String));
            }

            var roleClaims = roles.SelectMany(a => a.Claims);
            foreach (var claim in roleClaims)
            {
                identity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue, ClaimValueTypes.String));
            }

            var rolePermissions = roles.SelectMany(a => a.Permissions).Select(a => a.Name);
            var grantedPermissions = user.Permissions.Where(p => p.IsGranted).Select(a => a.Name);
            var deniedPermissions = user.Permissions.Where(p => !p.IsGranted).Select(a => a.Name);

            var permissions = rolePermissions.Union(grantedPermissions).Except(deniedPermissions);
            foreach (var permission in permissions)
            {
                identity.AddClaim(new Claim(DNTClaimTypes.Permission, permission, ClaimValueTypes.String));
            }

            //Todo: Set TenantId claim in MultiTenancy senarios     
            // claims.Add(new Claim(ConstantClaims.TenantId, user.TenantId.ToString(), ClaimValueTypes.Integer64,
            // _configuration.Value.Issuer));

            return new ClaimsPrincipal(identity);
        }
    }
}