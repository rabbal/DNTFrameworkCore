using System.Threading.Tasks;
using DNTFrameworkCore.Web.Caching;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Security
{
    /// <summary>
    /// More info: https://github.com/aspnet/Antiforgery/issues/116
    /// </summary>
    public class NoBrowserCacheAntiforgery : IAntiforgery
    {
        private readonly DefaultAntiforgery _defaultAntiforgery;

        public NoBrowserCacheAntiforgery(IOptions<AntiforgeryOptions> antiforgeryOptionsAccessor,
            IAntiforgeryTokenGenerator tokenGenerator,
            IAntiforgeryTokenSerializer tokenSerializer,
            IAntiforgeryTokenStore tokenStore,
            ILoggerFactory loggerFactory)
        {
            _defaultAntiforgery = new DefaultAntiforgery(antiforgeryOptionsAccessor,
                tokenGenerator,
                tokenSerializer,
                tokenStore,
                loggerFactory);
        }

        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        {
            var result = _defaultAntiforgery.GetAndStoreTokens(httpContext);
            httpContext.DisableBrowserCache();
            return result;
        }

        public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
        {
            return _defaultAntiforgery.GetTokens(httpContext);
        }

        public Task<bool> IsRequestValidAsync(HttpContext httpContext)
        {
            return _defaultAntiforgery.IsRequestValidAsync(httpContext);
        }

        public Task ValidateRequestAsync(HttpContext httpContext)
        {
            return _defaultAntiforgery.ValidateRequestAsync(httpContext);
        }

        public void SetCookieTokenAndHeader(HttpContext httpContext)
        {
            _defaultAntiforgery.SetCookieTokenAndHeader(httpContext);
        }
    }
}