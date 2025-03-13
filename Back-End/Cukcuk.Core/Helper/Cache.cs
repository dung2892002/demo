using Microsoft.Extensions.Caching.Memory;

namespace Cukcuk.Core.Helper
{
    public class Cache(IMemoryCache cache)
    {
        private readonly IMemoryCache _cache = cache;


        public void SaveDataToCache<T>(Guid id, List<T> datas)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            _cache.Set(id, datas, cacheEntryOptions);
        }

        public List<T>? GetFromCache<T>(Guid? id)
        {
            _cache.TryGetValue(id, out List<T>? data);
            return data;
        }

        public void RemoveCache(Guid id)
        {
            if (_cache.TryGetValue(id, out _))
            {
                _cache.Remove(id);
            }
        }

    }
}
