using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DNTFrameworkCore.TestWebApp.Application.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DNTFrameworkCore.TestWebApp.Authentication
{
    public interface ICookieValidator
    {
        Task ValidateAsync(CookieValidatePrincipalContext context);
    }

    public class CookieValidator : ICookieValidator
    {
        private readonly IUserManager _manager;
        public CookieValidator(IUserManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
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

            var user = await _manager.FindAsync(userId);
            if (!user.HasValue || user.Value.SerialNumber != serialNumberClaim.Value || !user.Value.IsActive)
            {
                // user has changed his/her password/permissions/roles/stat/IsActive
                await HandleUnauthorizedRequest(context);
            }

            await _manager.UpdateLastActivityDateAsync(user.Value);
        }

        private Task HandleUnauthorizedRequest(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}