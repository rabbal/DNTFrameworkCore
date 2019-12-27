using System;
using Microsoft.Extensions.Caching.Memory;

namespace DNTFrameworkCore.Caching
{
    /// <summary>
    /// ICacheService encapsulates IMemoryCache functionality.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
        /// Otherwise it will use the factory method to get the value and then inserts it.
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="factory"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="size">Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count of entries.</param>
        /// <typeparam name="T"></typeparam>
        T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1);

        /// <summary>
        /// Gets the key's value from the cache.
        /// </summary>
        T Get<T>(string cacheKey);

        /// <summary>
        /// Tries to get the key's value from the cache.
        /// </summary>
        bool TryGetValue<T>(string cacheKey, out T result);

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="size">Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count of entries.</param>
        /// <typeparam name="T"></typeparam>
        void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration, int size = 1);

        /// <summary>
        /// Adds a key-value to the cache.
        /// It will use the factory method to get the value and then inserts it.
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="factory"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="size">Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count of entries.</param>
        /// <typeparam name="T"></typeparam>
        void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1);

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="size">Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count of entries.</param>
        /// <typeparam name="T"></typeparam>
        void Add<T>(string cacheKey, T value, int size = 1);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        void Remove(string cacheKey);
    }

    /// <summary>
    /// Encapsulates IMemoryCache functionality.
    /// </summary>
    internal sealed class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        ///  Encapsulates IMemoryCache functionality.
        /// </summary>
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        /// Gets the key's value from the cache.
        /// </summary>
        public T Get<T>(string cacheKey)
        {
            return _memoryCache.Get<T>(cacheKey);
        }

        /// <summary>
        /// Tries to get the key's value from the cache.
        /// </summary>
        public bool TryGetValue<T>(string cacheKey, out T result)
        {
            return _memoryCache.TryGetValue(cacheKey, out result);
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// It will use the factory method to get the value and then inserts it.
        /// </summary>
        public void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
        {
            _memoryCache.Set(cacheKey, factory(), new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                Size = size // the size limit is the count of entries
            });
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        public void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration, int size = 1)
        {
            _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                Size = size // the size limit is the count of entries
            });
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        public void Add<T>(string cacheKey, T value, int size = 1)
        {
            _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
            {
                Size = size // the size limit is the count of entries
            });
        }

        /// <summary>
        /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
        /// Otherwise it will use the factory method to get the value and then inserts it.
        /// </summary>
        public T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
        {
            // locks get and set internally
            if (_memoryCache.TryGetValue<T>(cacheKey, out var result))
            {
                return result;
            }

            lock (TypeLock<T>.Lock)
            {
                if (_memoryCache.TryGetValue(cacheKey, out result))
                {
                    return result;
                }

                result = factory();
                _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = absoluteExpiration,
                    Size = size // the size limit is the count of entries
                });

                return result;
            }
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        private static class TypeLock<T>
        {
            public static object Lock { get; } = new object();
        }
    }
}