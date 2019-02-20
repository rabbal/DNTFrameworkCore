using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.TestAPI.Application.Identity;
using DNTFrameworkCore.TestAPI.Resources;
using DNTFrameworkCore.Web.Security;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public interface IAuthenticationService : IScopedDependency
    {
        Task<SignInResult> SignInAsync(string userName, string password);
        Task SignOutAsync();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserManager _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly IRoleManager _roleManager;
        private readonly IAntiForgeryCookieService _antiForgery;
        private readonly IOptionsSnapshot<TokenOptions> _configuration;
        private readonly IMessageLocalizer _localizer;
        private readonly IUserSession _session;

        public AuthenticationService(IUserManager userManager,
            ITokenManager tokenManager,
            IRoleManager roleManager,
            IAntiForgeryCookieService antiForgery,
            IOptionsSnapshot<TokenOptions> configuration,
            IMessageLocalizer localizer,
            IUserSession session)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _antiForgery = antiForgery ?? throw new ArgumentNullException(nameof(antiForgery));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _session = session ?? throw new ArgumentNullException(nameof(session));
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

            var claims = await GenerateClaimsAsync(userId);

            var token = await _tokenManager.BuildTokenAsync(userId, claims);

            _antiForgery.RegenerateAntiForgeryCookies(claims);

            return SignInResult.Success(token);
        }

        /// <summary>
        /// The Jwt implementation does not support "revoke OAuth token" (logout) by design.
        /// Delete the user's tokens from the database (revoke its bearer token)
        /// </summary>
        public async Task SignOutAsync()
        {
            await _tokenManager.RevokeTokensAsync(_session.UserId);

            _antiForgery.DeleteAntiForgeryCookies();
        }

        private async Task<IList<Claim>> GenerateClaimsAsync(long userId)
        {
            var userMaybe = await _userManager.FindIncludeClaimsAsync(userId);
            if (!userMaybe.HasValue) return new List<Claim>();

            var user = userMaybe.Value;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String,
                    _configuration.Value.Issuer),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String,
                    _configuration.Value.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                new Claim(DNTClaimTypes.UserId, user.Id.ToString(), ClaimValueTypes.Integer64,
                    _configuration.Value.Issuer),
                new Claim(DNTClaimTypes.UserName, user.UserName, ClaimValueTypes.String,
                    _configuration.Value.Issuer),
                new Claim(DNTClaimTypes.DisplayName, user.DisplayName, ClaimValueTypes.String,
                    _configuration.Value.Issuer),
                new Claim(DNTClaimTypes.SerialNumber, user.SerialNumber, ClaimValueTypes.String,
                    _configuration.Value.Issuer),
                new Claim(DNTClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String,
                    _configuration.Value.Issuer)
            };

            foreach (var claim in user.Claims)
            {
                claims.Add(new Claim(claim.ClaimType, claim.ClaimValue, ClaimValueTypes.String,
                    _configuration.Value.Issuer));
            }

            var roles = await _roleManager.FindUserRolesIncludeClaimsAsync(user.Id);
            foreach (var role in roles)
            {
                claims.Add(new Claim(DNTClaimTypes.Role, role.Name, ClaimValueTypes.String,
                    _configuration.Value.Issuer));
            }

            var roleClaims = roles.SelectMany(a => a.Claims);
            foreach (var claim in roleClaims)
            {
                claims.Add(new Claim(claim.ClaimType, claim.ClaimValue, ClaimValueTypes.String,
                    _configuration.Value.Issuer));
            }

            var rolePermissions = roles.SelectMany(a => a.Permissions).Select(a => a.Name);
            var grantedPermissions = user.Permissions.Where(p => p.IsGranted).Select(a => a.Name);
            var deniedPermissions = user.Permissions.Where(p => !p.IsGranted).Select(a => a.Name);

            var permissions = rolePermissions.Union(grantedPermissions).Except(deniedPermissions);
            foreach (var permission in permissions)
            {
                claims.Add(new Claim(DNTClaimTypes.Permission, permission, ClaimValueTypes.String,
                    _configuration.Value.Issuer));
            }

            //Todo: Set TenantId claim in MultiTenancy senarios     
            // claims.Add(new Claim(ConstantClaims.TenantId, user.TenantId.ToString(), ClaimValueTypes.Integer64,
            // _configuration.Value.Issuer));

            return claims;
        }
    }
}