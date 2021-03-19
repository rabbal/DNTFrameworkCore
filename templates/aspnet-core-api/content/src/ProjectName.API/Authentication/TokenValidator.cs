using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Runtime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ProjectName.Domain.Identity;

namespace ProjectName.API.Authentication
{
    public interface ITokenValidator : IScopedDependency
    {
        Task ValidateAsync(TokenValidatedContext context);
    }

    public class TokenValidator : ITokenValidator
    {
        private readonly ITokenService _token;
        private readonly DbSet<User> _users;

        public TokenValidator(ITokenService token, IDbContext dbContext)
        {
            Ensure.IsNotNull(dbContext, nameof(dbContext));

            _token = token ?? throw new ArgumentNullException(nameof(token));
            _users = dbContext.Set<User>();
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var principal = context.Principal;

            if (principal != null)
            {
                var claimsIdentity = principal.Identity as ClaimsIdentity;
                if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
                {
                    context.Fail("This is not our issued token. It has no claims.");
                    return;
                }

                var securityTokenClaim = claimsIdentity.FindFirst(UserClaimTypes.SecurityToken);
                if (securityTokenClaim == null)
                {
                    context.Fail("This is not our issued token. It has no securityToken.");
                    return;
                }

                var userIdString = claimsIdentity.FindFirst(UserClaimTypes.UserId)?.Value;
                if (!long.TryParse(userIdString, out var userId))
                {
                    context.Fail("This is not our issued token. It has no userId.");
                    return;
                }

                var user = await FindUserAsync(userId);
                if (!user.HasValue || user.Value.SecurityToken != securityTokenClaim.Value || !user.Value.IsActive)
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
        }

        private async Task<Maybe<User>> FindUserAsync(long userId)
        {
            return await _users
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == userId);
        }
    }
}