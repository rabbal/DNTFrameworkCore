using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Security
{
    public interface IAntiforgeryService
    {
        void RebuildCookies(IEnumerable<Claim> claims);
        void DeleteCookies();
    }

    public class AntiforgeryService : IAntiforgeryService
    {
        private const string XsrfTokenKey = "XSRF-TOKEN";

        private readonly IHttpContextAccessor _context;
        private readonly IAntiforgery _antiforgery;
        private readonly IOptions<AntiforgeryOptions> _options;

        public AntiforgeryService(
            IHttpContextAccessor context,
            IAntiforgery antiforgery,
            IOptions<AntiforgeryOptions> options)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void RebuildCookies(IEnumerable<Claim> claims)
        {
            var httpContext = _context.HttpContext;
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
            var tokens = _antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append(
                XsrfTokenKey,
                tokens.RequestToken,
                new CookieOptions
                {
                    HttpOnly = false // Now JavaScript is able to read the cookie
                });
        }

        public void DeleteCookies()
        {
            var cookies = _context.HttpContext.Response.Cookies;
            cookies.Delete(_options.Value.Cookie.Name);
            cookies.Delete(XsrfTokenKey);
        }
    }
}