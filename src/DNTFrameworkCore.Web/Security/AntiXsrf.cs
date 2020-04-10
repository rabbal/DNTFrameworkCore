using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Security
{
    public interface IAntiXsrf
    {
        /// <summary>
        /// Add XsrfToken in Response.Cookies with 'XSRF-TOKEN' name
        /// </summary>
        void AddToken(IEnumerable<Claim> claims, string authenticationType);
        /// <summary>
        /// Remove XsrfToken in Response.Cookies with 'XSRF-TOKEN' name
        /// </summary>
        void RemoveToken();
    }

    internal class AntiXsrf : IAntiXsrf
    {
        private const string XsrfToken = "XSRF-TOKEN";

        private readonly IHttpContextAccessor _context;
        private readonly IAntiforgery _antiforgery;
        private readonly IOptions<AntiforgeryOptions> _options;

        public AntiXsrf(
            IHttpContextAccessor context,
            IAntiforgery antiforgery,
            IOptions<AntiforgeryOptions> options)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void AddToken(IEnumerable<Claim> claims, string authenticationType)
        {
            var httpContext = _context.HttpContext;
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));
            var tokens = _antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append(
                XsrfToken,
                tokens.RequestToken,
                new CookieOptions
                {
                    HttpOnly = false // Now JavaScript is able to read the cookie
                });
        }

        public void RemoveToken()
        {
            var cookies = _context.HttpContext.Response.Cookies;
            cookies.Delete(_options.Value.Cookie.Name);
            cookies.Delete(XsrfToken);
        }
    }
}