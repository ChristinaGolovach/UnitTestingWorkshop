using System;
using System.Collections.Generic;
using System.Linq;
using CacheService.Storages;
using CacheService.Exceptions;

namespace CacheService
{
    public class CacheService<TKey, TItem> : ICacheService<TKey, TItem>
    {
        private const int defaultCacheCapacity = 7;
        private int cacheCapacity;
        private IStorage<TKey, TItem> cacheStorage;
        
        public CacheService(StorageFactory<TKey, TItem> storageFactory) 
            : this(defaultCacheCapacity, EqualityComparer<TKey>.Default, storageFactory) { }

        public CacheService(int capacity, StorageFactory<TKey, TItem> storageFactory) 
            : this(capacity, EqualityComparer<TKey>.Default, storageFactory) { }

        public CacheService(IEqualityComparer<TKey> keyEqualityComparer, StorageFactory<TKey, TItem> storageFactory) 
            : this(defaultCacheCapacity, keyEqualityComparer, storageFactory) { }

        public CacheService(int capacity, IEqualityComparer<TKey> keyEqualityComparer, StorageFactory<TKey, TItem> storageFactory)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(capacity)} should be more than zero.");
            }

            if (keyEqualityComparer == null)
            {
                throw new ArgumentNullException($"The {nameof(keyEqualityComparer)} can not be null.");
            }

            if (storageFactory == null)
            {
                throw new ArgumentNullException($"The {nameof(storageFactory)} can not be null.");
            }

            cacheStorage = storageFactory.CreateStorage(capacity, keyEqualityComparer);

            cacheCapacity = capacity;              
        }

        public void AddItem(TKey key, TItem item, int lifeTime)
        {
            CheckKey(key);

            if (lifeTime <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (cacheStorage.IsCachedItem(key))
            {
                cacheStorage.Remove(key);
            }

            if (cacheCapacity == cacheStorage.CachedItemCount)
            {
                RemoveLastAccessedItem();
            }

            cacheStorage.Add(key, item, lifeTime);
        }

        public TItem GetItem(TKey key)
        {
            CheckKey(key);

            CacheItemModel<TItem> cachedItem = null;

            if (!cacheStorage.IsCachedItem(key))
            {
                throw new CachedItemNotFoundException($"The data with key {key} is not found.");
            }

            cachedItem = cacheStorage.Get(key);

            if (cachedItem.IsExpired())
            {
                cacheStorage.Remove(key);

                throw new ExpirationTimeException($"Data with key {key} has expired.");
            }

            return cachedItem.Value;
        }

        private void RemoveLastAccessedItem()
        {
            DateTime minLastAccessItemTime = cacheStorage.GetAll().Min(item => item.Value.LastAccessTime);

            var lastAccessedItem = cacheStorage.GetAll().First(item => item.Value.LastAccessTime == minLastAccessItemTime);

            cacheStorage.Remove(lastAccessedItem.Key);
        }

        private void CheckKey(TKey key)
        {
            if(EqualityComparer<TKey>.Default.Equals(key, default(TKey)))
            {
                throw new ArgumentNullException($"The {nameof(key)} can not be null.");
            }
        }
    }
}
