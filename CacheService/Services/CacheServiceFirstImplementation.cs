using System;
using System.Collections.Generic;
using System.Linq;
using CacheService.Exceptions;

namespace CacheService.Services
{
    //cache storage (Dictionary in this case) within service and no ability to change it. 

    //public class CacheServiceFirstImplementation<TKey, TItem> : ICacheService<TKey, TItem>
    //{
    //    private const int defaultCacheCapacity = 7;
    //    private int cacheCapacity;
    //    private int cachedItemCount;
    //    private IDictionary<TKey, CacheItemModel<TItem>> cachedItems;

    //    public CacheServiceFirstImplementation() : this(defaultCacheCapacity, EqualityComparer<TKey>.Default) { }

    //    public CacheServiceFirstImplementation(int capacity) : this(capacity, EqualityComparer<TKey>.Default) { }

    //    public CacheServiceFirstImplementation(IEqualityComparer<TKey> keyEqualityComparer) : this(defaultCacheCapacity, keyEqualityComparer) { }

    //    public CacheServiceFirstImplementation(int capacity, IEqualityComparer<TKey> keyEqualityComparer)
    //    {
    //        if (capacity <= 0)
    //        {
    //            throw new ArgumentOutOfRangeException($"The {nameof(capacity)} should be more than zero.");
    //        }

    //        if (keyEqualityComparer == null)
    //        {
    //            throw new ArgumentNullException($"The {nameof(keyEqualityComparer)} can not be null.");
    //        }

    //        cacheCapacity = capacity;

    //        cachedItems = new Dictionary<TKey, CacheItemModel<TItem>>(capacity, keyEqualityComparer);
    //    }

    //    public void AddItem(TKey key, TItem item, int lifeTime)
    //    {
    //        CheckKey(key);

    //        if (lifeTime <= 0)
    //        {
    //            throw new ArgumentOutOfRangeException();
    //        }

    //        CacheItemModel<TItem> cachedItem = null;

    //        if (cachedItems.TryGetValue(key, out cachedItem))
    //        {
    //            UpdateItem(cachedItem, item, lifeTime);
    //        }

    //        if (cacheCapacity == cachedItemCount)
    //        {
    //            RemoveLastAccessedItem();
    //        }

    //        var cacheItem = new CacheItemModel<TItem> { Value = item, ExpirationTime = DateTime.Now.AddSeconds(lifeTime), LastAccessTime = DateTime.Now };

    //        cachedItems.Add(key, cacheItem);
    //        cachedItemCount++;
    //    }

    //    public TItem GetItem(TKey key)
    //    {
    //        CheckKey(key);

    //        CacheItemModel<TItem> cachedItem = null;

    //        if (!cachedItems.TryGetValue(key, out cachedItem))
    //        {
    //            throw new ArgumentException($"Data with key {key} does not exist in the cache.");
    //        }

    //        if (cachedItem.IsExpired())
    //        {
    //            RemoveItem(key);

    //            throw new ExpirationTimeException($"Data with key {key} have expired.");
    //        }

    //        cachedItem.LastAccessTime = DateTime.Now;

    //        return cachedItem.Value;
    //    }

    //    private void RemoveLastAccessedItem()
    //    {
    //        DateTime minLastAccessItemTime = cachedItems.Min(i => i.Value.LastAccessTime);

    //        var lastAccessedItem = cachedItems.First(i => i.Value.LastAccessTime == minLastAccessItemTime);

    //        RemoveItem(lastAccessedItem.Key);
    //    }

    //    private void RemoveItem(TKey key)
    //    {
    //        cachedItems.Remove(key);
    //        cachedItemCount--;
    //    }

    //    private void UpdateItem(CacheItemModel<TItem> cachedItem, TItem item, int lifeTime)
    //    {
    //        cachedItem.Value = item;
    //        cachedItem.LastAccessTime = DateTime.Now;
    //        cachedItem.ExpirationTime = DateTime.Now.AddSeconds(lifeTime);
    //    }

    //    private void CheckKey(TKey key)
    //    {
    //        // but if type will be nullable struct
    //        if (!typeof(TKey).IsValueType && ReferenceEquals(key, null))
    //        {
    //            throw new ArgumentNullException($"The {nameof(key)} can not be null.");
    //        }
    //    }
    //}
}
