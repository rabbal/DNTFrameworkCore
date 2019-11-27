using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public interface ITokenValidator : IScopedDependency
    {
        Task ValidateAsync(TokenValidatedContext context);
    }

    public class TokenValidator : ITokenValidator
    {
        private readonly ITokenService _token;
        private readonly IUnitOfWork _uow;

        public TokenValidator(ITokenService token, IUnitOfWork uow)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
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

            var serialNumberClaim = claimsIdentity.FindFirst(UserClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                context.Fail("This is not our issued token. It has no serial-number.");
                return;
            }

            var userIdString = claimsIdentity.FindFirst(UserClaimTypes.UserId).Value;
            if (!long.TryParse(userIdString, out var userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }

            var user = await FindUserAsync(userId);
            if (!user.HasValue || user.Value.SerialNumber != serialNumberClaim.Value || !user.Value.IsActive)
            {
                // user has changed his/her password/permissions/roles/stat/IsActive
                context.Fail("This token is expired. Please login again.");
                return;
            }

            if (!(context.SecurityToken is JwtSecurityToken token) ||
                string.IsNullOrWhiteSpace(token.RawData) ||
                !await _token.IsValidTokenAsync(userId, token.RawData))
            {
                context.Fail("This token is not in our database.");
            }
        }

        private async Task<Maybe<User>> FindUserAsync(long userId)
        {
            return await _uow.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == userId);
        }
    }
}