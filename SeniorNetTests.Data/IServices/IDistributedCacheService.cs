using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeniorNetTests.Services
{
    public interface IDistributedCacheService
    {
        TItem GetOrCreate<TItem>(Func<TItem> factory, params string[] keys);

        Task<TItem> GetOrCreateAsync<TItem>(Func<Task<TItem>> factory, params string[] keys);

        void Remove(params string[] keys);
    }
}
