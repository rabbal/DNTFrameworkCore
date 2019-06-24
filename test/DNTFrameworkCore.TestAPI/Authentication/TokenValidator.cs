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
        Task ValidateAsync(TokenValidatedContext uow);
    }

    public class TokenValidator : ITokenValidator
    {
        private readonly ITokenManager _token;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<User> _users;

        public TokenValidator(ITokenManager token, IUnitOfWork uow)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _users = uow.Set<User>();
        }

        public async Task ValidateAsync(TokenValidatedContext uow)
        {
            var principal = uow.Principal;

            var claimsIdentity = principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                uow.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(DNTClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                uow.Fail("This is not our issued token. It has no serial-number.");
                return;
            }

            var userIdString = claimsIdentity.FindFirst(DNTClaimTypes.UserId).Value;
            if (!long.TryParse(userIdString, out var userId))
            {
                uow.Fail("This is not our issued token. It has no user-id.");
                return;
            }

            var user = await FindUserAsync(userId);
            if (!user.HasValue || user.Value.SerialNumber != serialNumberClaim.Value || !user.Value.IsActive)
            {
                // user has changed his/her password/permissions/roles/stat/IsActive
                uow.Fail("This token is expired. Please login again.");
                return;
            }

            if (!(uow.SecurityToken is JwtSecurityToken accessToken) ||
                string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !await _token.IsValidTokenAsync(userId, accessToken.RawData))
            {
                uow.Fail("This token is not in our database.");
                return;
            }

            //TODO: Concurrency Issue when current user edit own account in user management
            //await UpdateLastActivityDateAsync(user.Value);
        }

        public async Task<Maybe<User>> FindUserAsync(long userId)
        {
            return await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}