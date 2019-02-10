using System;
using System.Collections.Generic;
using CacheService.Exceptions;

namespace CacheService.Storages
{
    public class InMemoryStorage<TKey, TValue> : IStorage<TKey, TValue>
    {
        private IDictionary<TKey, CacheItemModel<TValue>> cachedItems;

        public int CachedItemCount { get; private set; }

        public InMemoryStorage(int capacity, IEqualityComparer<TKey> keyEqualityComparer)
        {
            cachedItems = new Dictionary<TKey, CacheItemModel<TValue>>(capacity, keyEqualityComparer);
        }

        public bool IsCachedItem(TKey key)
        {
            return cachedItems.ContainsKey(key);
        }

        public IEnumerable<KeyValuePair<TKey, CacheItemModel<TValue>>> GetAll()
        {
            return cachedItems;
        }

        public CacheItemModel<TValue> Get(TKey key)
        {
            CacheItemModel<TValue> cachedItem;

            if (!cachedItems.TryGetValue(key, out cachedItem))
            {
                throw new CachedItemNotFoundException($"The data with key {key} is not found.");
            }

            cachedItem.LastAccessTime = DateTime.Now;

            return cachedItem;
        }

        public void Add(TKey key, TValue value, int lifeTime)
        {
            var item = new CacheItemModel<TValue>() { Value = value, ExpirationTime = DateTime.Now.AddSeconds(lifeTime), LastAccessTime = DateTime.Now };            
            cachedItems.Add(key, item);
            CachedItemCount++;
        }

        public void Remove(TKey key)
        {
            cachedItems.Remove(key);
            CachedItemCount--;
        }
    }
}
