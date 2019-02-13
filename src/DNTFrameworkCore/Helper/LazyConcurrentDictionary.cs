using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DNTFrameworkCore.Helper
{
    public class LazyConcurrentDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _concurrentDictionary;

        public LazyConcurrentDictionary()
        {
            _concurrentDictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        public LazyConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            _concurrentDictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>(comparer);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            var lazyResult = _concurrentDictionary.GetOrAdd(key,
                k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));
            return lazyResult.Value;
        }
    }
}