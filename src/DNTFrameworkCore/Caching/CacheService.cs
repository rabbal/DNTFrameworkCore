using System;
using System.Threading;
using System.Threading.Tasks;
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
        T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
        /// Otherwise it will use the factory method to get the value and then inserts it.
        /// </summary>
        Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, DateTimeOffset absoluteExpiration);

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
        void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Adds a key-value to the cache.
        /// It will use the factory method to get the value and then inserts it.
        /// </summary>
        void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        void Add<T>(string cacheKey, T value);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        void Remove(string cacheKey);
    }

    /// <summary>
    /// Encapsulates IMemoryCache functionality.
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        /// <summary>
        /// فقط یک ترد امکان دسترسی به کد را داشته باشد
        /// </summary>
        /// <returns></returns>
        private static readonly SemaphoreSlim _locker = new SemaphoreSlim(1, 1);

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
        public void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration)
        {
            _memoryCache.Set(cacheKey, factory(), absoluteExpiration);
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        public void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration)
        {
            _memoryCache.Set(cacheKey, value, absoluteExpiration);
        }

        /// <summary>
        /// Adds a key-value to the cache.
        /// </summary>
        public void Add<T>(string cacheKey, T value)
        {
            _memoryCache.Set(cacheKey, value);
        }

        /// <summary>
        /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
        /// Otherwise it will use the factory method to get the value and then inserts it.
        /// </summary>
        public T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration)
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

                _memoryCache.Set(cacheKey, result, absoluteExpiration);

                return result;
            }
        }


        /// <summary>
        /// A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
        /// Otherwise it will use the factory method to get the value and then inserts it.
        /// </summary>
        public async Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory,
            DateTimeOffset absoluteExpiration)
        {
            // locks get and set internally
            if (_memoryCache.TryGetValue<T>(cacheKey, out var result))
            {
                return result;
            }

            await _locker.WaitAsync();

            try
            {
                if (_memoryCache.TryGetValue(cacheKey, out result))
                {
                    return result;
                }

                result = await factory();

                _memoryCache.Set(cacheKey, result, absoluteExpiration);

                return result;
            }
            finally
            {
                _locker.Release();
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
            public static readonly object Lock = new object();
        }
    }
}