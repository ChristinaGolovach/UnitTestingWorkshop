using System;
using System.Collections.Generic;
using CacheService.Exceptions;

namespace CacheService.Storages
{
    /// <summary>
    /// Imlements <see cref="IStorage{TKey, TItem}"/>
    /// </summary>
    /// <typeparam name="TKey">The type of key of item.</typeparam>
    /// <typeparam name="TValue">The type of value of item.</typeparam>
    public class InMemoryStorage<TKey, TValue> : IStorage<TKey, TValue>
    {
        private IDictionary<TKey, CacheItemModel<TValue>> cachedItems;

        /// <inheritdoc/>
        public int CachedItemCount { get; private set; }


        /// <summary>
        /// nitializes a new instance of the <see cref="InMemoryStorage"/> class with specified <see cref="IEqualityComparer{T}"/> and capacity.
        /// </summary>
        /// <param name="capacity">The size of storage in elements.</param>
        /// <param name="keyEqualityComparer">A <see cref="IEqualityComparer{T}"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="keyEqualityComparer"/>is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="capacity"/>is less one.</exception>
        public InMemoryStorage(int capacity, IEqualityComparer<TKey> keyEqualityComparer)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(capacity)} can not be lezz one.");
            }

            if (keyEqualityComparer == null)
            {
                throw new ArgumentNullException($"The {nameof(keyEqualityComparer)} can not null.");
            }

            cachedItems = new Dictionary<TKey, CacheItemModel<TValue>>(capacity, keyEqualityComparer);
        }

        /// <inheritdoc/>
        public bool IsCachedItem(TKey key)
        {
            CheckKey(key);

            return cachedItems.ContainsKey(key);
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<TKey, CacheItemModel<TValue>>> GetAll()
        {
            return cachedItems;
        }

        /// <inheritdoc/>
        public CacheItemModel<TValue> Get(TKey key)
        {
            CheckKey(key);

            CacheItemModel<TValue> cachedItem;

            if (!cachedItems.TryGetValue(key, out cachedItem))
            {
                throw new CachedItemNotFoundException($"The data with key {key} is not found.");
            }

            cachedItem.LastAccessTime = DateTime.Now;

            return cachedItem;
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value, int lifeTime)
        {
            CheckKey(key);

            if (lifeTime <= 0)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(lifeTime)} can not be less one.");
            }

            if (cachedItems.ContainsKey(key))
            {
                throw new ArgumentException($"The item with key {key} already exists.");
            }

            var item = new CacheItemModel<TValue>() { Value = value, ExpirationTime = DateTime.Now.AddSeconds(lifeTime), LastAccessTime = DateTime.Now };            
            cachedItems.Add(key, item);
            CachedItemCount++;
        }

        /// <inheritdoc/>
        public void Remove(TKey key)
        {
            CheckKey(key);

            if (!cachedItems.ContainsKey(key))
            {
                throw new ArgumentException($"The item with {key} not found.");
            }

            cachedItems.Remove(key);
            CachedItemCount--;
        }

        private void CheckKey(TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(key, default(TKey)))
            {
                throw new ArgumentNullException($"The {nameof(key)} can not be null.");
            }
        }
    }
}
