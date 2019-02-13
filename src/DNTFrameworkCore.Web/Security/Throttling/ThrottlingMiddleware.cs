using System;
using System.Net;
using System.Threading.Tasks;
using DNTFrameworkCore.Web.Http;
using DNTFrameworkCore.Web.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Security.Throttling
{
    /// <summary>
    /// AntiDos Middleware
    /// </summary>
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private IOptionsSnapshot<ThrottlingOptions> _antiDosConfig;

        /// <summary>
        /// AntiDos Middleware
        /// </summary>
        public ThrottlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// AntiDos Middleware Pipeline
        /// </summary>
        public async Task Invoke(
             HttpContext context,
             IOptionsSnapshot<ThrottlingOptions> antiDosConfig,
             IThrottlingFirewall antiDosFirewall)
        {
            _antiDosConfig = antiDosConfig ?? throw new ArgumentNullException(nameof(antiDosConfig));
            if (_antiDosConfig.Value == null)
            {
                throw new ArgumentNullException(nameof(antiDosConfig), "Please add AntiDosConfig to your appsettings.json file.");
            }

            var requestInfo = getHeadersInfo(context);

            var validationResult = antiDosFirewall.ShouldBlockClient(requestInfo);
            if (validationResult.ShouldBlockClient)
            {
                antiDosFirewall.LogIt(validationResult.ThrottleInfo, requestInfo);
                addResetHeaders(context, validationResult.ThrottleInfo);
                await blockClient(context);
                return;
            }
            await _next(context);
        }

        private ThrottlingFirewallRequestInfo getHeadersInfo(HttpContext context)
        {
            return new ThrottlingFirewallRequestInfo
            {
                IP = context.GetIp(),
                UserAgent = context.GetUserAgent(),
                UrlReferrer = context.GetReferrerUri(),
                RawUrl = context.GetRawUrl(),
                IsLocal = context.IsLocal(),
                RequestHeaders = context.Request.Headers
            };
        }

        private Task blockClient(HttpContext context)
        {
            // see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            return context.Response.WriteAsync(_antiDosConfig.Value.ErrorMessage);
        }

        private void addResetHeaders(HttpContext context, ThrottleInfo throttleInfo)
        {
            if (throttleInfo == null)
            {
                return;
            }
            context.Response.Headers["X-RateLimit-Limit"] = _antiDosConfig.Value.AllowedRequests.ToString();
            var requestsRemaining = Math.Max(_antiDosConfig.Value.AllowedRequests - throttleInfo.RequestsCount, 0);
            context.Response.Headers["X-RateLimit-Remaining"] = requestsRemaining.ToString();
            context.Response.Headers["X-RateLimit-Reset"] = throttleInfo.ExpiresAt.ToUnixTimeSeconds().ToString();
            context.Response.Headers["Retry-After"] = context.Response.Headers["X-RateLimit-Reset"];
        }
    }
}