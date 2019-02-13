using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Http
{
     /// <summary>
    /// Url Normalization Service Extensions
    /// </summary>
    public static class UrlNormalizationServiceExtensions
    {
        /// <summary>
        /// Adds IUrlNormalizationService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddUrlNormalizationService(this IServiceCollection services)
        {
            services.AddTransient<IUrlNormalizationService, UrlNormalizationService>();
            return services;
        }
    }

    /// <summary>
    /// Url Normalization Service
    /// </summary>
    public interface IUrlNormalizationService
    {
        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        Task<bool> AreTheSameUrlsAsync(string url1, string url2, bool findRedirectUrl);

        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        Task<bool> AreTheSameUrlsAsync(Uri uri1, Uri uri2, bool findRedirectUrl);

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        Task<string> NormalizeUrlAsync(Uri uri, bool findRedirectUrl);

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        Task<string> NormalizeUrlAsync(string url, bool findRedirectUrl);
    }

    /// <summary>
    /// Url Normalization Service
    /// </summary>
    public class UrlNormalizationService : IUrlNormalizationService
    {
        private readonly IRedirectUrlFinderService _locationFinder;

        /// <summary>
        /// Url Normalization Service
        /// </summary>
        public UrlNormalizationService(IRedirectUrlFinderService locationFinder)
        {
            _locationFinder = locationFinder ?? throw new ArgumentNullException(nameof(locationFinder));
        }

        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        public async Task<bool> AreTheSameUrlsAsync(string url1, string url2, bool findRedirectUrl)
        {
            url1 = await NormalizeUrlAsync(url1, findRedirectUrl);
            url2 = await NormalizeUrlAsync(url2, findRedirectUrl);
            return url1.Equals(url2);
        }

        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        public async Task<bool> AreTheSameUrlsAsync(Uri uri1, Uri uri2, bool findRedirectUrl)
        {
            var url1 = await NormalizeUrlAsync(uri1, findRedirectUrl);
            var url2 = await NormalizeUrlAsync(uri2, findRedirectUrl);
            return url1.Equals(url2);
        }

        private static readonly string[] DefaultDirectoryIndexes =
        {
            "default.asp",
            "default.aspx",
            "index.htm",
            "index.html",
            "index.php"
        };

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        public async Task<string> NormalizeUrlAsync(Uri uri, bool findRedirectUrl)
        {
            if (findRedirectUrl)
            {
                uri = await _locationFinder.GetRedirectUrlAsync(uri);
            }
            var url = UrlToLower(uri);
            url = LimitProtocols(url);
            url = RemoveDefaultDirectoryIndexes(url);
            url = RemoveTheFragment(url);
            url = RemoveDuplicateSlashes(url);
            url = AddWww(url);
            url = RemoveFeedburnerPart1(url);
            url = RemoveFeedburnerPart2(url);
            return RemoveTrailingSlashAndEmptyQuery(url);
        }

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        public Task<string> NormalizeUrlAsync(string url, bool findRedirectUrl)
        {
            return NormalizeUrlAsync(new Uri(url), findRedirectUrl);
        }

        private static string RemoveFeedburnerPart1(string url)
        {
            var idx = url.IndexOf("utm_source=", StringComparison.Ordinal);
            return idx == -1 ? url : url.Substring(0, idx - 1);
        }

        private static string RemoveFeedburnerPart2(string url)
        {
            var idx = url.IndexOf("utm_medium=", StringComparison.Ordinal);
            return idx == -1 ? url : url.Substring(0, idx - 1);
        }

        private static string AddWww(string url)
        {
            if (new Uri(url).Host.Split('.').Length == 2 && !url.Contains("://www."))
            {
                return url.Replace("://", "://www.");
            }
            return url;
        }

        private static string RemoveDuplicateSlashes(string url)
        {
            var path = new Uri(url).AbsolutePath;
            return path.Contains("//") ? url.Replace(path, path.Replace("//", "/")) : url;
        }

        private static string LimitProtocols(string url)
        {
            return new Uri(url).Scheme == "https" ? url.Replace("https://", "http://") : url;
        }

        private static string RemoveTheFragment(string url)
        {
            var fragment = new Uri(url).Fragment;
            return string.IsNullOrWhiteSpace(fragment) ? url : url.Replace(fragment, string.Empty);
        }

        private static string UrlToLower(Uri uri)
        {
            return WebUtility.UrlDecode(uri.AbsoluteUri.ToLowerInvariant());
        }

        private static string RemoveTrailingSlashAndEmptyQuery(string url)
        {
            return url
                    .TrimEnd('?')
                    .TrimEnd('/');
        }

        private static string RemoveDefaultDirectoryIndexes(string url)
        {
            foreach (var index in DefaultDirectoryIndexes)
            {
                if (url.EndsWith(index))
                {
                    url = url.TrimEnd(index.ToCharArray());
                    break;
                }
            }
            return url;
        }
    }
}