using System;

namespace CacheService
{
    //internal 
    public class CacheItemModel<TValue>
    {
        public TValue Value { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime ExpirationTime { get; set; }

        public CacheItemModel()
        {

        }

        public CacheItemModel(TValue value, int time)
        {
            Value = value;
            LastAccessTime = DateTime.Now;
            ExpirationTime = DateTime.Now.AddSeconds(time);
        }

        public bool IsExpired() => DateTime.Now >= ExpirationTime;
    }
}
