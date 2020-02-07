using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorNetTests.Services
{
    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _distributedCache;

        private readonly ConcurrentDictionary<string, string> Keys = new ConcurrentDictionary<string, string>();

        public DistributedCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public TItem GetOrCreate<TItem>(Func<TItem> factory, params string[] keys)
        {
            TItem result;

            if (keys.Length <= 0)
            {
                result = factory.Invoke();
            }
            else
            {
                string key = string.Join(",", keys).Trim(',');

                if (!Keys.ContainsKey(key))
                {
                    string parent = string.Join(",", keys.Take(keys.Length - 1)).Trim(',');

                    Keys.TryAdd(key, parent);
                }

                result = _distributedCache.Get(key, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Configs.CacheExpiration);
                    return factory.Invoke();
                });
            }

            return result;
        }

        public Task<TItem> GetOrCreateAsync<TItem>(Func<Task<TItem>> factory, params string[] keys)
        {
            throw new NotImplementedException();
        }

        public void Remove(params string[] keys)
        {
            throw new NotImplementedException();
        }
    }
}
