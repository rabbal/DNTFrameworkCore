using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DNTFrameworkCore.Web.Network;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Http
{
    /// <summary>
    /// Redirect Url Finder Service Extensions
    /// </summary>
    public static class RedirectUrlFinderServiceExtensions
    {
        /// <summary>
        /// Adds IRedirectUrlFinderService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddRedirectUrlFinderService(this IServiceCollection services)
        {
            services.AddTransient<IRedirectUrlFinderService, RedirectUrlFinderService>();
            return services;
        }
    }
    
    /// <summary>
    /// Redirect Url Finder Service
    /// </summary>
    public interface IRedirectUrlFinderService
    {
        /// <summary>
        /// Finds the actual hidden URL after multiple redirects.
        /// </summary>
        Task<string> GetRedirectUrlAsync(string siteUrl, int maxRedirects = 20);

        /// <summary>
        /// Finds the actual hidden URL after multiple redirects.
        /// </summary>
        Task<Uri> GetRedirectUrlAsync(Uri siteUri, int maxRedirects = 20);
    }
    
    /// <summary>
    /// Redirect Url Finder Service
    /// </summary>
    public class RedirectUrlFinderService : IRedirectUrlFinderService
    {
        private readonly HttpClient _client = new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
            Proxy = null,
            AllowAutoRedirect = false,
            CookieContainer = new CookieContainer()
        })
        {
            Timeout = TimeSpan.FromMinutes(3)
        };

        private readonly IMemoryCache _cache;
        private const string CachePrefix = "LocationFinder::";
        private readonly ILogger<RedirectUrlFinderService> _logger;

        static RedirectUrlFinderService()
        {
#if !NETSTANDARD1_6
            // Default is 2 minutes: https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
            ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            // Increases the concurrent outbound connections
            ServicePointManager.DefaultConnectionLimit = 1024;
#endif
        }

        /// <summary>
        /// Redirect Url Finder Service
        /// </summary>
        public RedirectUrlFinderService(IMemoryCache cacheService, ILogger<RedirectUrlFinderService> logger)
        {
            _cache = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Finds the actual hidden URL after multiple redirects.
        /// </summary>
        public async Task<string> GetRedirectUrlAsync(string siteUrl, int maxRedirects = 20)
        {
            var uri = await GetRedirectUrlAsync(new Uri(siteUrl), maxRedirects);
            return uri.OriginalString;
        }

        /// <summary>
        /// Finds the actual hidden URL after multiple redirects.
        /// </summary>
        public async Task<Uri> GetRedirectUrlAsync(Uri siteUri, int maxRedirects = 20)
        {
            var redirectUri = siteUri;

            try
            {
                if (_cache.TryGetValue($"{CachePrefix}{siteUri}", out string outUrl))
                {
                    return new Uri(outUrl);
                }

                _client.DefaultRequestHeaders.Add("User-Agent", typeof(RedirectUrlFinderService).Namespace);
                _client.DefaultRequestHeaders.Add("Keep-Alive", "true");
                _client.DefaultRequestHeaders.Referrer = siteUri;

                var hops = 1;
                do
                {
                    var webResp = await _client.GetAsync(redirectUri, HttpCompletionOption.ResponseHeadersRead);
                    if (webResp == null)
                    {
                        return CacheReturn(siteUri, siteUri);
                    }

                    switch (webResp.StatusCode)
                    {
                        case HttpStatusCode.Found: // 302 = HttpStatusCode.Redirect
                        case HttpStatusCode.Moved: // 301 = HttpStatusCode.MovedPermanently
                            redirectUri = webResp.Headers.Location;
                            if (!redirectUri.IsAbsoluteUri)
                            {
                                var leftPartAuthority = siteUri.GetComponents(UriComponents.Scheme | UriComponents.StrongAuthority, UriFormat.Unescaped);
                                redirectUri = new Uri($"{leftPartAuthority}{redirectUri}");
                            }
                            break;
                        case HttpStatusCode.Unauthorized:
                        case HttpStatusCode.Forbidden:
                            // fine! they have banned this server, but the link is correct!
                            return CacheReturn(siteUri, redirectUri);
                        case HttpStatusCode.OK:
                            return CacheReturn(siteUri, redirectUri);

                        default:
                            webResp.EnsureSuccessStatusCode();
                            break;
                    }

                    hops++;
                } while (hops <= maxRedirects);

                throw new InvalidOperationException("Too many redirects detected.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("LocationFinderService error", ex, $"Couldn't find redirect of {siteUri}");
            }
            catch (Exception ex) when (ex.IsNetworkError())
            {
                _logger.LogError("LocationFinderService error", ex, $"Couldn't find redirect of {siteUri}");
            }

            return CacheReturn(siteUri, redirectUri);
        }

        private Uri CacheReturn(Uri originalUrl, Uri redirectUrl)
        {
            _cache.Set($"{CachePrefix}{originalUrl}", redirectUrl,
                DateTimeOffset.UtcNow.AddMinutes(15));
            return redirectUrl;
        }
    }
}