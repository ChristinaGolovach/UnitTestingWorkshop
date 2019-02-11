using System;
using System.Collections.Generic;
using System.Linq;
using CacheService.Storages;
using CacheService.Exceptions;

namespace CacheService
{
    /// <summary>
    /// Imlements <see cref="ICacheService{TKey, TItem}"/>
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TItem">The type of item</typeparam>
    public class CacheService<TKey, TItem> : ICacheService<TKey, TItem>
    {
        private const int defaultCacheCapacity = 7;
        private int cacheCapacity;
        private IStorage<TKey, TItem> cacheStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService{TKey, TItem}"/> class with specified <see cref="StorageFactory{TKey, TValue}"/> 
        /// and default cacpacity of cache is 7 elements and default <see cref="EqualityComparer{T}"/>.
        /// </summary>
        /// <param name="storageFactory">The specified <see cref="StorageFactory{TKey, TValue}"/>where items will be stored.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="storageFactory"/>is null.</exception>
        public CacheService(StorageFactory<TKey, TItem> storageFactory) 
            : this(defaultCacheCapacity, EqualityComparer<TKey>.Default, storageFactory) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService{TKey, TItem}"/> class with specified <see cref="StorageFactory{TKey, TValue}"/> 
        /// and with specified <paramref name="capacity"/> nd default <see cref="EqualityComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">The storage size in elements.</param>
        /// <param name="storageFactory">The specified <see cref="StorageFactory{TKey, TValue}"/>where items will be stored.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="storageFactory"/>is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="capacity"/>is less one.</exception>
        public CacheService(int capacity, StorageFactory<TKey, TItem> storageFactory) 
            : this(capacity, EqualityComparer<TKey>.Default, storageFactory) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService{TKey, TItem}"/> class with specified <see cref="StorageFactory{TKey, TValue}"/> 
        /// and with specified <see cref="IEqualityComparer{T}"/> and default cacpacity of the cache is 7 elements. 
        /// </summary>
        /// <param name="keyEqualityComparer">A <see cref="IEqualityComparer{T}"/>.</param>
        /// <param name="storageFactory">A <see cref="StorageFactory{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="storageFactory"/>is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="keyEqualityComparer"/>is null.</exception>
        public CacheService(IEqualityComparer<TKey> keyEqualityComparer, StorageFactory<TKey, TItem> storageFactory) 
            : this(defaultCacheCapacity, keyEqualityComparer, storageFactory) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService{TKey, TItem}"/> class with specified <see cref="StorageFactory{TKey, TValue}"/> 
        /// and with specified <see cref="IEqualityComparer{T}"/> and with specified cacpacity of the cache. 
        /// </summary>
        /// <param name="capacity">The storage size in elements.</param>
        /// <param name="keyEqualityComparer">A <see cref="IEqualityComparer{T}"/>.</param>
        /// <param name="storageFactory">A <see cref="StorageFactory{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="storageFactory"/>is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="keyEqualityComparer"/>is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="capacity"/>is less one.</exception> 
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

        /// <summary>
        /// Add an item in the cache.
        /// </summary>
        /// <param name="key">The key for cache.</param>
        /// <param name="item">The item for cache.</param>
        /// <param name="lifeTime">Lifetime of item in the cache in seconds.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/>is null.</exception>
        /// <exception cref = "ArgumentOutOfRangeException" > The < paramref name="key"/>is null.</exception>
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
        /// <summary>
        /// Get an item from the cache by the given key.
        /// </summary>
        /// <param name="key">The key of desired value.</param>
        /// <returns>The item from cache.</returns>
        /// <exception cref="CachedItemNotFoundException">The item with <paramref name="key"/> not found.</exception>
        /// <exception cref="ExpirationTimeException">The lifetime of item with <paramref name="key"/> is espired.</exception>
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
