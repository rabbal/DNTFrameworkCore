//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using DNTFrameworkCore.GuardToolkit;
//using DNTFrameworkCore.MultiTenancy;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Primitives;
//
//namespace DNTFrameworkCore.Web.MultiTenancy
// {
//   public abstract class MemoryCacheTenantResolver : ITenantResolver
//     {
//         protected readonly IMemoryCache Cache;
//         protected readonly ILogger Logger;
//         protected readonly MemoryCacheTenantResolverOptions Options;
//
//         protected MemoryCacheTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory)
//             : this(cache, loggerFactory, new MemoryCacheTenantResolverOptions())
//         {
//         }
//
//         protected MemoryCacheTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory, MemoryCacheTenantResolverOptions options)
//         {
//             Guard.ArgumentNotNull(cache, nameof(cache));
//             Guard.ArgumentNotNull(loggerFactory, nameof(loggerFactory));
//             Guard.ArgumentNotNull(options, nameof(options));
//
//             Cache = cache;
//             Logger = loggerFactory.CreateLogger<MemoryCacheTenantResolver>();
//             Options = options;
//         }
//
//         protected virtual MemoryCacheEntryOptions CreateCacheEntryOptions()
//         {
//             return new MemoryCacheEntryOptions()
//                 .SetSlidingExpiration(new TimeSpan(1, 0, 0));
//         }
//
//         protected virtual void DisposeTenantContext(object cacheKey, TenantContext tenantContext)
//         {
//             if (tenantContext == null) return;
//            
//             Logger.LogDebug("Disposing TenantContext:{id} instance with key \"{cacheKey}\".", tenantContext.Id, cacheKey);
//            
//             tenantContext.Dispose();
//         }
//
//         protected abstract string GetContextIdentifier(HttpContext context);
//         protected abstract IEnumerable<string> GetTenantIdentifiers(TenantContext context);
//         protected abstract Task<TenantContext> ResolveAsync(HttpContext context);
//
//         async Task<TenantContext> ITenantResolver.ResolveAsync(HttpContext context)
//         {
//             Guard.ArgumentNotNull(context, nameof(context));
//
//             // Obtain the key used to identify cached tenants from the current request
//             var cacheKey = GetContextIdentifier(context);
//
//             if (cacheKey == null)
//             {
//                 return null;
//             }
//
//             if (!(Cache.Get(cacheKey) is TenantContext tenantContext))
//             {
//                 Logger.LogDebug("TenantContext not present in cache with key \"{cacheKey}\". Attempting to resolve.", cacheKey);
//                 tenantContext = await ResolveAsync(context);
//
//                 if (tenantContext == null) return tenantContext;
//                
//                 var tenantIdentifiers = GetTenantIdentifiers(tenantContext);
//
//                 if (tenantIdentifiers != null)
//                 {
//                     var cacheEntryOptions = GetCacheEntryOptions();
//
//                     Logger.LogDebug("TenantContext:{id} resolved. Caching with keys \"{tenantIdentifiers}\".", tenantContext.Id, tenantIdentifiers);
//
//                     foreach (var identifier in tenantIdentifiers)
//                     {
//                         Cache.Set(identifier, tenantContext, cacheEntryOptions);
//                     }
//                 }
//             }
//             else
//             {
//                 Logger.LogDebug("TenantContext:{id} retrieved from cache with key \"{cacheKey}\".", tenantContext.Id, cacheKey);
//             }
//
//             return tenantContext;
//         }
//
//         private MemoryCacheEntryOptions GetCacheEntryOptions()
//         {
//             var cacheEntryOptions = CreateCacheEntryOptions();
//
//             if (Options.EvictAllEntriesOnExpiry)
//             {
//                 var tokenSource = new CancellationTokenSource();
//
//                 cacheEntryOptions
//                     .RegisterPostEvictionCallback(
//                         (key, value, reason, state) =>
//                         {
//                             tokenSource.Cancel();
//                         })
//                     .AddExpirationToken(new CancellationChangeToken(tokenSource.Token));
//             }
//
//             if (Options.DisposeOnEviction)
//             {
//                 cacheEntryOptions
//                     .RegisterPostEvictionCallback(
//                         (key, value, reason, state) =>
//                         {
//                             DisposeTenantContext(key, value as TenantContext);
//                         });
//             }
//
//             return cacheEntryOptions;
//         }
//     }
// }