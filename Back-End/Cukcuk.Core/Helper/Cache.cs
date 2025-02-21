using Microsoft.Extensions.Caching.Memory;

namespace Cukcuk.Core.Helper
{
    public class Cache(IMemoryCache cache)
    {
        private readonly IMemoryCache _cache = cache;


        public void SaveDataToCache<T>(Guid id, List<T> datas)
        {
            _cache.Set(id, datas);
        }

        public List<T>? GetFromCache<T>(Guid? id)
        {
            _cache.TryGetValue(id, out List<T>? data);
            return data;
        }

    }
}
