using Microsoft.Extensions.Caching.Distributed;
using SeniorNetTests.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeniorNetTests.Services
{
    public class DistributedCacheService : IDistributedCacheService
    {
        public static int CacheExpiration => 15;

        private readonly IDistributedCache _distributedCache;

        private readonly ConcurrentDictionary<string, string> Keys = new ConcurrentDictionary<string, string>();

        private readonly DistributedCacheEntryOptions cacheOptions;

        public DistributedCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Configs.CacheExpiration)
            };
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

                var cache = _distributedCache.GetString(key);

                if (string.IsNullOrWhiteSpace(cache))
                {
                    result = factory.Invoke();
                    cache = JsonSerializer.Serialize(result);
                    _distributedCache.SetString(key, cache, cacheOptions);
                }
                else
                {
                    result = JsonSerializer.Deserialize<TItem>(cache);
                }
            }

            return result;
        }

        public async  Task<TItem> GetOrCreateAsync<TItem>(Func<Task<TItem>> factory, params string[] keys)
        {
            TItem result;

            if (keys.Length <= 0)
            {
                result = await factory.Invoke();
            }
            else
            {
                string key = string.Join(",", keys).Trim(',');

                if (!Keys.ContainsKey(key))
                {
                    string parent = string.Join(",", keys.Take(keys.Length - 1)).Trim(',');

                    Keys.TryAdd(key, parent);
                }

                var cache = await _distributedCache.GetStringAsync(key);

                if (string.IsNullOrWhiteSpace(cache))
                {
                    result = await factory.Invoke();
                    cache = JsonSerializer.Serialize(result);
                    _distributedCache.SetString(key, cache, cacheOptions);
                }
                else
                {
                    result = JsonSerializer.Deserialize<TItem>(cache);
                }
            }

            return result;
        }

        public void Remove(params string[] keys)
        {
            if (keys.Length <= 0)
            {
                return;
            }

            RemoveRelatedKeys(string.Join(",", keys).Trim(','));
        }

        private void RemoveRelatedKeys(string key)
        {
            _distributedCache.Remove(key);
            Keys.TryRemove(key, out _);

            foreach (var item in Keys.Where(k => k.Value.Length >= key.Length && k.Value.StartsWith(key, StringComparison.InvariantCulture)))
            {
                RemoveRelatedKeys(item.Key);
            }
        }
    }
}
