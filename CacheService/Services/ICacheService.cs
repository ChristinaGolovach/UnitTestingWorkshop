namespace CacheService
{
    /// <summary>
    /// Represents a simple cache service.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TItem">The type of item</typeparam>
    public interface ICacheService<TKey, TItem>
    {
        /// <summary>
        /// Add an item in the cache.
        /// </summary>
        /// <param name="key">The key for cache.</param>
        /// <param name="item">The item for cache.</param>
        /// <param name="lifeTime">Lifetime of item in the cache in seconds.</param>
        void AddItem(TKey key, TItem item, int lifeTime);

        /// <summary>
        /// Get an item from the cache by the given key.
        /// </summary>
        /// <param name="key">The key of desired value.</param>
        /// <returns>The item from cache.</returns>
        TItem GetItem(TKey key);
    }
}
