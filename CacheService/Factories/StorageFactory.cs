using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheService.Storages;

namespace CacheService
{
    public abstract class StorageFactory<TKey, TValue>
    {
        public abstract IStorage<TKey, TValue> CreateStorage(int capacity, IEqualityComparer<TKey> keyEqualityComparer);
    }
}
