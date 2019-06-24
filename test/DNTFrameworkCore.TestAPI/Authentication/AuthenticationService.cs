using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using DNTFrameworkCore.TestAPI.Resources;
using DNTFrameworkCore.Web.Security;
using Microsoft.EntityFrameworkCore;
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
        private readonly ITokenManager _token;
        private readonly IUnitOfWork _uow;
        private readonly IAntiforgeryService _antiforgery;
        private readonly IOptionsSnapshot<TokenOptions> _options;
        private readonly IMessageLocalizer _localizer;
        private readonly IUserPassword _password;
        private readonly IUserSession _session;
        private readonly DbSet<User> _users;
        private readonly DbSet<Role> _roles;

        public AuthenticationService(
            ITokenManager token,
            IUnitOfWork uow,
            IAntiforgeryService antiforgery,
            IOptionsSnapshot<TokenOptions> options,
            IMessageLocalizer localizer,
            IUserPassword password,
            IUserSession session)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _session = session ?? throw new ArgumentNullException(nameof(session));

            _users = _uow.Set<User>();
            _roles = _uow.Set<Role>();
        }

        public async Task<SignInResult> SignInAsync(string userName, string password)
        {
            var maybe = await FindUserByNameAsync(userName);
            if (!maybe.HasValue)
            {
                return SignInResult.Fail(_localizer["SignIn.Messages.Failure"]);
            }

            var user = maybe.Value;

            if (_password.VerifyHashedPassword(user.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return SignInResult.Fail(_localizer["SignIn.Messages.Failure"]);
            }

            if (!user.IsActive)
            {
                return SignInResult.Fail(_localizer["SignIn.Messages.IsNotActive"]);
            }

            var userId = user.Id;

            var claims = await BuildClaimsAsync(userId);
            var token = await _token.BuildTokenAsync(userId, claims);
            _antiforgery.RebuildCookies(claims);

            return SignInResult.Ok(token);
        }

        /// <summary>
        /// The Jwt implementation does not support "revoke OAuth token" (logout) by design.
        /// Delete the user's tokens from the database (revoke its bearer token)
        /// </summary>
        public async Task SignOutAsync()
        {
            await _token.RevokeTokensAsync(_session.UserId);

            _antiforgery.DeleteCookies();
        }

        private async Task<IList<Claim>> BuildClaimsAsync(long userId)
        {
            var maybe = await FindUserIncludeClaimsAsync(userId);
            if (!maybe.HasValue) return new List<Claim>();

            var user = maybe.Value;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String,
                    _options.Value.Issuer),
                new Claim(JwtRegisteredClaimNames.Iss, _options.Value.Issuer, ClaimValueTypes.String,
                    _options.Value.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64, _options.Value.Issuer),
                new Claim(DNTClaimTypes.UserId, user.Id.ToString(), ClaimValueTypes.Integer64,
                    _options.Value.Issuer),
                new Claim(DNTClaimTypes.UserName, user.UserName, ClaimValueTypes.String,
                    _options.Value.Issuer),
                new Claim(DNTClaimTypes.DisplayName, user.DisplayName, ClaimValueTypes.String,
                    _options.Value.Issuer),
                new Claim(DNTClaimTypes.SerialNumber, user.SerialNumber, ClaimValueTypes.String,
                    _options.Value.Issuer),
                new Claim(DNTClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String,
                    _options.Value.Issuer)
            };

            foreach (var claim in user.Claims)
            {
                claims.Add(new Claim(claim.ClaimType, claim.ClaimValue, ClaimValueTypes.String,
                    _options.Value.Issuer));
            }

            var roles = await FindUserRolesIncludeClaimsAsync(user.Id);
            foreach (var role in roles)
            {
                claims.Add(new Claim(DNTClaimTypes.Role, role.Name, ClaimValueTypes.String,
                    _options.Value.Issuer));
            }

            var roleClaims = roles.SelectMany(a => a.Claims);
            foreach (var claim in roleClaims)
            {
                claims.Add(new Claim(claim.ClaimType, claim.ClaimValue, ClaimValueTypes.String,
                    _options.Value.Issuer));
            }

            var rolePermissions = roles.SelectMany(a => a.Permissions).Select(a => a.Name);
            var grantedPermissions = user.Permissions.Where(p => p.IsGranted).Select(a => a.Name);
            var deniedPermissions = user.Permissions.Where(p => !p.IsGranted).Select(a => a.Name);

            var permissions = rolePermissions.Union(grantedPermissions).Except(deniedPermissions);
            foreach (var permission in permissions)
            {
                claims.Add(new Claim(DNTClaimTypes.Permission, permission, ClaimValueTypes.String,
                    _options.Value.Issuer));
            }

            //Todo: Set TenantId claim in MultiTenancy scenarios     
            // claims.Add(new Claim(DNTClaimTypes.TenantId, user.TenantId.ToString(), ClaimValueTypes.Integer64,
            // _options.Value.Issuer));

            //Todo: Set BranchId claim in MultiBranch scenarios     
            // claims.Add(new Claim(DNTClaimTypes.BranchId, user.BranchId.ToString(), ClaimValueTypes.Integer64,
            // _options.Value.Issuer));

            return claims;
        }

        private async Task<Maybe<User>> FindUserIncludeClaimsAsync(long userId)
        {
            return await _users.Include(u => u.Permissions)
                .AsNoTracking()
                .Include(u => u.Claims)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        private async Task<IList<Role>> FindUserRolesIncludeClaimsAsync(long userId)
        {
            var query = from role in _roles
                        from userRoles in role.Users
                        where userRoles.UserId == userId
                        select role;

            return await query
                .AsNoTracking()
                .Include(r => r.Permissions)
                .Include(r => r.Claims)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        private async Task<Maybe<User>> FindUserByNameAsync(string userName)
        {
            var normalizedUserName = userName.ToUpperInvariant();

            return await _users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName);
        }
    }
}