using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DNTFrameworkCore.Web.Http
{
      /// <summary>
    /// Domain Helper Extensions
    /// </summary>
    public static class DomainHelperExtensions
    {
        private static readonly Lazy<List<string>> TldsBuilder =
             new Lazy<List<string>>(DefaultTlds, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Determines whether uri1 and uri2 have the same domain.
        /// </summary>
        public static bool HaveTheSameDomain(this Uri uri1, Uri uri2)
        {
            var domain2 = uri2.GetUrlDomain().Domain;
            var domain1 = uri1.GetUrlDomain().Domain;
            return domain2.Equals(domain1, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether uri1 and uri2 have the same domain.
        /// </summary>
        public static bool HaveTheSameDomain(this string uri1, string uri2)
        {
            return HaveTheSameDomain(new Uri(uri1), new Uri(uri2));
        }

        /// <summary>
        /// Determines whether the url has no extension.
        /// </summary>
        public static bool IsMvcPage(this Uri url)
        {
            return string.IsNullOrWhiteSpace(url.GetUriExtension());
        }

        /// <summary>
        /// Determines whether the url has no extension.
        /// </summary>
        public static bool IsMvcPage(this string url)
        {
            return IsMvcPage(new Uri(url));
        }

        /// <summary>
        /// Returns the extension of the uri.
        /// </summary>
        public static string GetUriExtension(this Uri uri)
        {
            try
            {
                return Path.GetExtension(uri.PathAndQuery);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the extension of the uri.
        /// </summary>
        public static string GetUriExtension(this string uri)
        {
            return GetUriExtension(new Uri(uri));
        }

        /// <summary>
        /// Returns the SubDomain of the uri.
        /// </summary>
        public static string GetSubDomain(this Uri url)
        {
            if (url.HostNameType != UriHostNameType.Dns)
                return null;

            var host = url.Host.TrimEnd('.');
            if (host.Split('.').Length <= 2)
                return null;

            var lastIndex = host.LastIndexOf(".", StringComparison.Ordinal);
            var index = host.LastIndexOf(".", lastIndex - 1, StringComparison.Ordinal);
            return host.Substring(0, index);
        }

        /// <summary>
        /// Returns the SubDomain of the uri.
        /// </summary>
        public static string GetSubDomain(this string url)
        {
            return GetSubDomain(new Uri(url));
        }

        /// <summary>
        /// Returns the host part without its SubDomain.
        /// </summary>
        public static string GetHostWithoutSubDomain(this Uri url)
        {
            var subdomain = GetSubDomain(url);
            var host = url.Host.TrimEnd(new[] { '.' });
            if (subdomain != null)
            {
                host = host.Replace(string.Format("{0}.", subdomain), string.Empty);
            }
            return host;
        }

        /// <summary>
        /// Returns the host part without its SubDomain.
        /// </summary>
        public static string GetHostWithoutSubDomain(this string url)
        {
            return GetHostWithoutSubDomain(new Uri(url));
        }

        /// <summary>
        /// Returns the domain part of the url.
        /// </summary>
        public static (string Domain, bool HasBestMatch) GetUrlDomain(this string url)
        {
            return GetUrlDomain(new Uri(url));
        }

        /// <summary>
        /// Returns the domain part of the url.
        /// </summary>
        public static (string Domain, bool HasBestMatch) GetUrlDomain(this Uri url)
        {
            if (url == null) return (string.Empty, false);
            var dotBits = url.Host.Split('.');
            switch (dotBits.Length)
            {
                case 1:
                case 2:
                    return (url.Host, false); //eg http://localhost/blah.php = "localhost"
            }

            var bestMatch = "";
            foreach (var tld in Tlds)
            {
                if (url.Host.EndsWith(tld, StringComparison.OrdinalIgnoreCase))
                {
                    if (tld.Length > bestMatch.Length) bestMatch = tld;
                }
            }
            if (string.IsNullOrEmpty(bestMatch))
                return (url.Host, false); //eg http://domain.com/blah = "domain.com"

            //add the domain name onto tld
            var bestBits = bestMatch.Split('.');
            var inputBits = url.Host.Split('.');
            var getLastBits = bestBits.Length + 1;
            bestMatch = "";
            for (var c = inputBits.Length - getLastBits; c < inputBits.Length; c++)
            {
                if (bestMatch.Length > 0) bestMatch += ".";
                bestMatch += inputBits[c];
            }
            return (bestMatch, true);
        }

        /// <summary>
        /// Tld Patterns
        /// </summary>
        public static List<string> Tlds
        {
            get { return TldsBuilder.Value; }
        }

        /// <summary>
        /// Determines whether the `referrer` has the same host or domain as `url`.
        /// </summary>
        public static bool IsLocalReferrer(this Uri referrer, Uri url)
        {
            return referrer.Host.TrimEnd('.').Equals(url.Host.TrimEnd('.'), StringComparison.OrdinalIgnoreCase)
                || HaveTheSameDomain(referrer, url);
        }

        /// <summary>
        /// Determines whether the `referrer` has the same host or domain as `url`.
        /// </summary>
        public static bool IsLocalReferrer(this string referrer, string url)
        {
            return IsLocalReferrer(new Uri(referrer), new Uri(url));
        }

        /// <summary>
        /// Determines whether the `destUri` has the same domain as `siteRootUrl`.
        /// </summary>
        public static bool IsReferrerToThisSite(this Uri destUri, string siteRootUrl)
        {
            if (destUri == null || string.IsNullOrWhiteSpace(siteRootUrl))
            {
                return false;
            }

            var siteDomain = GetUrlDomain(siteRootUrl).Domain;
            var destDomain = GetUrlDomain(destUri).Domain;
            return destDomain.Equals(siteDomain, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the `destUri` has the same domain as `siteRootUrl`.
        /// </summary>
        public static bool IsReferrerToThisSite(this string destUri, string siteRootUrl)
        {
            return IsReferrerToThisSite(new Uri(destUri), siteRootUrl);
        }

        private static List<string> DefaultTlds()
        {
            var tlds = new List<string>();
            tlds.AddRange(TldPatterns.EXACT);
            tlds.AddRange(TldPatterns.UNDER);
            tlds.AddRange(TldPatterns.EXCLUDED);
            return tlds;
        }

        /// <summary>
        /// Path.Combine for URLs
        /// </summary>
        public static string CombineUrl(this string baseUrl, string relativeUrl)
        {
            var baseUri = new UriBuilder(baseUrl);

            if (Uri.TryCreate(baseUri.Uri, relativeUrl, out var newUri))
                return newUri.ToString();
            throw new InvalidOperationException($"Unable to combine {baseUrl} with {relativeUrl}.");
        }
    }
}