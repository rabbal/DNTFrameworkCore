using System;
using System.Linq;
using System.Net;
using DNTFrameworkCore.Web.Network;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTools;

namespace DNTFrameworkCore.Web.Security.Throttling
{
    /// <summary>
    /// Anti Dos Firewall Extensions
    /// </summary>
    public static class AntiDosFirewallExtensions
    {
        /// <summary>
        /// Adds IAntiDosFirewall to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddAntiDosFirewall(this IServiceCollection services)
        {
            services.AddScoped<IThrottlingFirewall, ThrottlingFirewall>();
            return services;
        }
    }

    /// <summary>
    /// Anti Dos Firewall Service
    /// </summary>
    public interface IThrottlingFirewall
    {
        /// <summary>
        /// Such as `google` or `bing`.
        /// </summary>
        bool IsGoodBot(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Such as `asafaweb`.
        /// </summary>
        (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) IsBadBot(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Is from remote localhost
        /// </summary>
        bool IsFromRemoteLocalhost(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Such as `HTTP_ACUNETIX_PRODUCT`.
        /// </summary>
        (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) HasUrlAttackVectors(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Such as `HTTP_ACUNETIX_PRODUCT`.
        /// </summary>
        (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBanBotHeaders(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Such as `asafaweb`.
        /// </summary>
        (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBanUserAgent(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Should block client based on its info?
        /// </summary>
        (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBlockClient(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Should block client based on its IP?
        /// </summary>
        (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBanIp(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Is this a dos attack?
        /// </summary>
        (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) IsDosAttack(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Returns cache's expirations date
        /// </summary>
        DateTimeOffset GetCacheExpiresAt();

        /// <summary>
        /// Returns cache's key
        /// </summary>
        string GetCacheKey(ThrottlingFirewallRequestInfo requestInfo);

        /// <summary>
        /// Logs a warning
        /// </summary>
        void LogIt(ThrottleInfo throttleInfo, ThrottlingFirewallRequestInfo requestInfo);
    }

    /// <summary>
    /// Anti Dos Firewall
    /// </summary>
    public class ThrottlingFirewall : IThrottlingFirewall
    {
        private readonly IOptionsSnapshot<ThrottlingOptions> _options;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ThrottlingFirewall> _logger;

        /// <summary>
        /// Anti Dos Firewall
        /// </summary>
        public ThrottlingFirewall(
            IOptionsSnapshot<ThrottlingOptions> options,
            IMemoryCache cache,
            ILogger<ThrottlingFirewall> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.Value == null)
            {
                throw new ArgumentNullException(nameof(options), "Please add ThrottlingOptions to your appsettings.json file.");
            }

            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Such as `google` or `bing`.
        /// </summary>
        public bool IsGoodBot(ThrottlingFirewallRequestInfo requestInfo)
        {
            if (string.IsNullOrWhiteSpace(requestInfo.UserAgent))
            {
                return false;
            }

            if (_options.Value.GoodBotsUserAgents == null || !_options.Value.GoodBotsUserAgents.Any())
            {
                return true;
            }

            return _options.Value.GoodBotsUserAgents
                       .Any(goodBot => requestInfo.UserAgent.IndexOf(goodBot, StringComparison.OrdinalIgnoreCase) != -1);
        }

        /// <summary>
        /// Such as `asafaweb`.
        /// </summary>
        public (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) IsBadBot(ThrottlingFirewallRequestInfo requestInfo)
        {
            var shouldBanBotHeader = ShouldBanBotHeaders(requestInfo);
            if (shouldBanBotHeader.ShouldBlockClient)
            {
                return (true, shouldBanBotHeader.ThrottleInfo);
            }

            var shouldBanUserAgent = ShouldBanUserAgent(requestInfo);
            if (shouldBanUserAgent.ShouldBlockClient)
            {
                return (true, shouldBanUserAgent.ThrottleInfo);
            }

            return (false, null);
        }

        /// <summary>
        /// Is from remote localhost
        /// </summary>
        public bool IsFromRemoteLocalhost(ThrottlingFirewallRequestInfo requestInfo)
        {
            if (requestInfo.UrlReferrer == null)
            {
                return false;
            }

            if (requestInfo.UrlReferrer.Host.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
            {
                return false;
            }

            return !requestInfo.IP.IsLocalIp();
        }

        /// <summary>
        /// Such as `HTTP_ACUNETIX_PRODUCT`.
        /// </summary>
        public (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) HasUrlAttackVectors(ThrottlingFirewallRequestInfo requestInfo)
        {
            if (string.IsNullOrWhiteSpace(requestInfo.RawUrl))
            {
                return (false, null);
            }

            if (requestInfo.RawUrl.EndsWith(".php", StringComparison.OrdinalIgnoreCase))
            {
                return (true, new ThrottleInfo
                {
                    ExpiresAt = GetCacheExpiresAt(),
                    RequestsCount = _options.Value.AllowedRequests,
                    BanReason = $"{requestInfo.RawUrl} ends with .php and this an ASP.NET Core site!"
                });
            }

            if (_options.Value.UrlAttackVectors == null || !_options.Value.UrlAttackVectors.Any())
            {
                return (false, null);
            }

            var vector = _options.Value.UrlAttackVectors
                        .FirstOrDefault(attackVector => requestInfo.RawUrl.IndexOf(attackVector, StringComparison.OrdinalIgnoreCase) != -1);
            if (!string.IsNullOrWhiteSpace(vector))
            {
                return (true, new ThrottleInfo
                {
                    ExpiresAt = GetCacheExpiresAt(),
                    RequestsCount = _options.Value.AllowedRequests,
                    BanReason = $"UrlAttackVector: {vector}."
                });
            }

            return (false, null);
        }

        /// <summary>
        /// Such as `HTTP_ACUNETIX_PRODUCT`.
        /// </summary>
        public (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBanBotHeaders(ThrottlingFirewallRequestInfo requestInfo)
        {
            if (_options.Value.BadBotsRequestHeaders == null ||
                !_options.Value.BadBotsRequestHeaders.Any() ||
                requestInfo.RequestHeaders == null)
            {
                return (false, null);
            }

            foreach (var headerkey in requestInfo.RequestHeaders.Keys)
            {
                var headerValue = requestInfo.RequestHeaders[headerkey];
                if (string.IsNullOrWhiteSpace(headerValue))
                {
                    continue;
                }

                var botHeader = _options.Value.BadBotsRequestHeaders
                                           .FirstOrDefault(badBotHeader => headerValue.ToString().IndexOf(badBotHeader, StringComparison.OrdinalIgnoreCase) != -1 ||
                                                                headerkey.IndexOf(badBotHeader, StringComparison.OrdinalIgnoreCase) != -1);
                if (!string.IsNullOrWhiteSpace(botHeader))
                {
                    return (true, new ThrottleInfo
                    {
                        ExpiresAt = GetCacheExpiresAt(),
                        RequestsCount = _options.Value.AllowedRequests,
                        BanReason = $"BadBotRequestHeader: {botHeader}."
                    });
                }
            }

            return (false, null);
        }

        /// <summary>
        /// Such as `asafaweb`.
        /// </summary>
        public (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBanUserAgent(ThrottlingFirewallRequestInfo requestInfo)
        {
            if (string.IsNullOrWhiteSpace(requestInfo.UserAgent))
            {
                return (false, null); // for ping-backs validations
            }

            if (_options.Value.BadBotsUserAgents == null || !_options.Value.BadBotsUserAgents.Any())
            {
                return (false, null);
            }

            var userAgent = _options.Value.BadBotsUserAgents
                                 .FirstOrDefault(badBot => requestInfo.UserAgent.IndexOf(badBot, StringComparison.OrdinalIgnoreCase) != -1);
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                return (true, new ThrottleInfo
                {
                    ExpiresAt = GetCacheExpiresAt(),
                    RequestsCount = _options.Value.AllowedRequests,
                    BanReason = $"BadBotUserAgent: {userAgent}."
                });
            }

            var isLocal = IsFromRemoteLocalhost(requestInfo);
            if (isLocal)
            {
                return (true, new ThrottleInfo
                {
                    ExpiresAt = GetCacheExpiresAt(),
                    RequestsCount = _options.Value.AllowedRequests,
                    BanReason = $"IsFromRemoteLocalhost."
                });
            }

            return (false, null);
        }

        /// <summary>
        /// Should block client based on its info?
        /// </summary>
        public (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBlockClient(ThrottlingFirewallRequestInfo requestInfo)
        {
            var shouldBanIpResult = ShouldBanIp(requestInfo);
            if (shouldBanIpResult.ShouldBlockClient)
            {
                return (true, shouldBanIpResult.ThrottleInfo);
            }

            if (_options.Value.IgnoreLocalHost && requestInfo.IsLocal)
            {
                return (false, null);
            }

            var hasUrlAttackVectorsResult = HasUrlAttackVectors(requestInfo);
            if (hasUrlAttackVectorsResult.ShouldBlockClient)
            {
                return (true, hasUrlAttackVectorsResult.ThrottleInfo);
            }

            var isBadBotResult = IsBadBot(requestInfo);
            if (isBadBotResult.ShouldBlockClient)
            {
                return (true, isBadBotResult.ThrottleInfo);
            }

            if (IsGoodBot(requestInfo))
            {
                return (false, null);
            }

            var isDosAttackResult = IsDosAttack(requestInfo);
            if (isDosAttackResult.ShouldBlockClient)
            {
                return (true, isDosAttackResult.ThrottleInfo);
            }

            return (false, null);
        }

        /// <summary>
        /// Should block client based on its IP?
        /// </summary>
        public (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) ShouldBanIp(ThrottlingFirewallRequestInfo requestInfo)
        {
            if (_options.Value.BannedIPAddressRanges == null || !_options.Value.BannedIPAddressRanges.Any())
            {
                return (false, null);
            }

            var iPAddress = IPAddress.Parse(requestInfo.IP);

            foreach (var range in _options.Value.BannedIPAddressRanges)
            {
                var ipRange = IPAddressRange.Parse(range);
                if (ipRange.Contains(iPAddress))
                {
                    return (true, new ThrottleInfo
                    {
                        ExpiresAt = GetCacheExpiresAt(),
                        RequestsCount = _options.Value.AllowedRequests,
                        BanReason = $"IP: {requestInfo.IP} is in the `{range}` range."
                    });
                }
            }

            return (false, null);
        }

        /// <summary>
        /// Is this a dos attack?
        /// </summary>
        public (bool ShouldBlockClient, ThrottleInfo ThrottleInfo) IsDosAttack(ThrottlingFirewallRequestInfo requestInfo)
        {
            var key = GetCacheKey(requestInfo);
            var expiresAt = GetCacheExpiresAt();
            if (!_cache.TryGetValue<ThrottleInfo>(key, out var clientThrottleInfo))
            {
                clientThrottleInfo = new ThrottleInfo { RequestsCount = 1, ExpiresAt = expiresAt };
                _cache.Set(key, clientThrottleInfo, expiresAt);
                return (false, clientThrottleInfo);
            }

            if (clientThrottleInfo.RequestsCount > _options.Value.AllowedRequests)
            {
                clientThrottleInfo.BanReason = "IsDosAttack";
                _cache.Set(key, clientThrottleInfo, expiresAt);
                return (true, clientThrottleInfo);
            }

            clientThrottleInfo.RequestsCount++;
            _cache.Set(key, clientThrottleInfo, expiresAt);
            return (false, clientThrottleInfo);
        }

        /// <summary>
        /// Returns cache's expirations date
        /// </summary>
        public DateTimeOffset GetCacheExpiresAt()
        {
            return DateTimeOffset.UtcNow.AddMinutes(_options.Value.DurationMin);
        }

        /// <summary>
        /// Returns cache's key
        /// </summary>
        public string GetCacheKey(ThrottlingFirewallRequestInfo requestInfo)
        {
            return $"__AntiDos__{requestInfo.IP}";
        }

        /// <summary>
        /// Logs a warning
        /// </summary>
        public void LogIt(ThrottleInfo throttleInfo, ThrottlingFirewallRequestInfo requestInfo)
        {
            if (throttleInfo.IsLogged)
            {
                return;
            }

            _logger.LogWarning($"Banned IP: ${requestInfo.IP}, UserAgent: {requestInfo.UserAgent}. {throttleInfo}");
            throttleInfo.IsLogged = true;
            _cache.Set(GetCacheKey(requestInfo), throttleInfo, GetCacheExpiresAt());
        }
    }
}