using System;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Tenancy.Options
{
    /// <summary>
    /// Tenant aware options cache
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    internal sealed class TenantOptionsCache<TOptions> : IOptionsMonitorCache<TOptions>
        where TOptions : class
    {
        /// <summary>
        /// Caches stored in memory
        /// </summary>
        private readonly TenantOptionsDictionary<TOptions> _cache = new TenantOptionsDictionary<TOptions>();

        private readonly ITenantSession _tenantSession;

        public TenantOptionsCache(ITenantSession tenantSession)
        {
            _tenantSession = tenantSession;
        }

        public void Clear()
        {
            _cache.Get(_tenantSession.TenantId).Clear();
        }

        public TOptions GetOrAdd(string name, Func<TOptions> createOptions)
        {
            return _cache.Get(_tenantSession.TenantId).GetOrAdd(name, createOptions);
        }

        public bool TryAdd(string name, TOptions options)
        {
            return _cache.Get(_tenantSession.TenantId)
                .TryAdd(name, options);
        }

        public bool TryRemove(string name)
        {
            return _cache.Get(_tenantSession.TenantId)
                .TryRemove(name);
        }
    }
}