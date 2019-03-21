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
                await handleUnauthorizedRequest(context);
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                // this is not our issued cookie
                await handleUnauthorizedRequest(context);
                return;
            }

            var userIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                // this is not our issued cookie
                await handleUnauthorizedRequest(context);
                return;
            }

            var user = await _manager.FindAsync(userId).ConfigureAwait(false);
            if (!user.HasValue || user.Value.SerialNumber != serialNumberClaim.Value || !user.Value.IsActive)
            {
                // user has changed his/her password/permissions/roles/stat/IsActive
                await handleUnauthorizedRequest(context);
            }

            await _manager.UpdateLastActivityDateAsync(user.Value).ConfigureAwait(false);
        }

        private Task handleUnauthorizedRequest(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}