using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.TestAPI.Application.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public interface ITokenValidator : IScopedDependency
    {
        Task ValidateAsync(TokenValidatedContext context);
    }

    public class TokenValidator : ITokenValidator
    {
        private readonly IUserManager _userManager;
        private readonly ITokenManager _tokenManager;

        public TokenValidator(IUserManager userManager, ITokenManager tokenManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var principal = context.Principal;

            var claimsIdentity = principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(DNTClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                context.Fail("This is not our issued token. It has no serial-number.");
                return;
            }

            var userIdString = claimsIdentity.FindFirst(DNTClaimTypes.UserId).Value;
            if (!long.TryParse(userIdString, out var userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }

            var user = await _userManager.FindAsync(userId);
            if (!user.HasValue || user.Value.SerialNumber != serialNumberClaim.Value || !user.Value.IsActive)
            {
                // user has changed his/her password/permissions/roles/stat/IsActive
                context.Fail("This token is expired. Please login again.");
                return;
            }

            if (!(context.SecurityToken is JwtSecurityToken accessToken) ||
                string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !await _tokenManager.IsValidTokenAsync(userId, accessToken.RawData))
            {
                context.Fail("This token is not in our database.");
                return;
            }

            //TODO: Concurrency Issue when current user edit own account in user management
            await _userManager.UpdateLastActivityDateAsync(user.Value);
        }
    }
}