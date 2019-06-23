using System;
using System.Security.Claims;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Extensions
{
    public static class PrincipalExtensions
    {
        public static Maybe<string> FindFirstValue(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var claim = principal.FindFirst(claimType);
            return claim?.Value;
        }
    }
}