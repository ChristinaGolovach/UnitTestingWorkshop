using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheService.Storages
{
    public interface IStorage<TKey, TValue> 
    {
        int CachedItemCount { get; }

        bool IsCachedItem(TKey key);

        IEnumerable<KeyValuePair<TKey,CacheItemModel<TValue>>> GetAll();

        CacheItemModel<TValue> Get(TKey key);

        void Add(TKey key, TValue value, int lifeTime);

        void Remove(TKey key);
    }
}
