using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Authentication
{
    public interface ICookieValidator
    {
        Task ValidateAsync(CookieValidatePrincipalContext context);
    }

    public class CookieValidator : ICookieValidator
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<User> _users;

        public CookieValidator(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _users = uow.Set<User>();
        }
        public async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                // this is not our issued cookie
                await HandleUnauthorizedRequest(context);
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                // this is not our issued cookie
                await HandleUnauthorizedRequest(context);
                return;
            }

            var userIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!long.TryParse(userIdString, out var userId))
            {
                // this is not our issued cookie
                await HandleUnauthorizedRequest(context);
                return;
            }

            var user = await FindUserAsync(userId);
            if (!user.HasValue || user.Value.SerialNumber != serialNumberClaim.Value || !user.Value.IsActive)
            {
                // user has changed his/her password/permissions/roles/stat/IsActive
                await HandleUnauthorizedRequest(context);
            }

            // await UpdateLastActivityDateAsync(user.Value);
        }

        private Task HandleUnauthorizedRequest(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<Maybe<User>> FindUserAsync(long userId)
        {
            return await _users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
        }
    }
}