using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheService.Storages;

namespace CacheService
{
    public class InMemoryStorageFactory<TKey, TValue> : StorageFactory<TKey, TValue>
    {
        public override IStorage<TKey, TValue> CreateStorage(int capacity, IEqualityComparer<TKey> keyEqualityComparer)
        {
            return new InMemoryStorage<TKey, TValue>(capacity, keyEqualityComparer);
        }
    }
}
