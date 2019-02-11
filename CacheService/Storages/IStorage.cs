using System.Collections.Generic;

namespace CacheService.Storages
{
    /// <summary>
    /// Represents a storage of cached items.
    /// </summary>
    /// <typeparam name="TKey">The type of key of item.</typeparam>
    /// <typeparam name="TValue">The type of value of item.</typeparam>
    public interface IStorage<TKey, TValue> 
    {
        /// <summary>
        /// Gets count items in storage.
        /// </summary>
        int CachedItemCount { get; }

        /// <summary>
        /// Checks if the item in cache.
        /// </summary>
        /// <param name="key">Key of the item to be checked.</param>
        /// <returns>True  - if item in cache.</returns>
        bool IsCachedItem(TKey key);

        /// <summary>
        /// Returns all items from the cache.
        /// </summary>
        /// <returns>Stored dadta in cache.</returns>
        IEnumerable<KeyValuePair<TKey,CacheItemModel<TValue>>> GetAll();

        /// <summary>
        /// Gets item from storage by key.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns>The item from storage.</returns>
        CacheItemModel<TValue> Get(TKey key);

        /// <summary>
        /// Adds an item to storage.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="value">The value of item. </param>
        /// <param name="lifeTime">Lifetime of item in seconds.</param>
        void Add(TKey key, TValue value, int lifeTime);

        /// <summary>
        /// Removes an item from storage by key.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        void Remove(TKey key);
    }
}
