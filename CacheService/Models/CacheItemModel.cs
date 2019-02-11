using System;

namespace CacheService
{
    /// <summary>
    /// Represents a cache item model.
    /// </summary>
    /// <typeparam name="TValue">The type of cached item.</typeparam>
    public class CacheItemModel<TValue>
    {
        /// <summary>
        /// Gets and sets value of cached item.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets and sets the last time access of cached item.
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Gets and sets the expiration time of cached item.
        /// </summary>
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// Check the expiration time of cached item.
        /// </summary>
        /// <returns>False - if time is up.</returns>
        public bool IsExpired() => DateTime.Now >= ExpirationTime;
    }
}
