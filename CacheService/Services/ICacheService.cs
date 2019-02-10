using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheService
{
    public interface ICacheService<TKey, TItem>
    {
        void AddItem(TKey key, TItem item, int lifeTime);
        TItem GetItem(TKey key);
    }
}
