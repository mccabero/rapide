using Microsoft.Extensions.Caching.Memory;

namespace Rapide.Common.Helpers
{
    public class CacheService
    {
        protected IMemoryCache MemoryCache { get; set; }

        public CacheService(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
        }

        public T? TryGetCache<T>(string cacheKey)
        {
            MemoryCache.TryGetValue<T>(cacheKey.ToLower(), out T value);
            return value;
        }

        public void SetSlidingCache(string cacheKey, object cachedObj)
        {
            object cachedObj2 = cachedObj;
            TimeSpan slidingExpiration = TimeSpan.FromSeconds(5.0);
            TimeSpan absoluteExpiration = TimeSpan.FromSeconds(10.0);
            MemoryCache.GetOrCreate(cacheKey.ToLower(), delegate (ICacheEntry entry)
            {
                entry.SetSlidingExpiration(slidingExpiration);
                entry.AbsoluteExpirationRelativeToNow = absoluteExpiration;
                return cachedObj2;
            });
        }

        public void SetPermanentCache(string cacheKey, object cachedObj)
        {
            object cachedObj2 = cachedObj;
            MemoryCache.GetOrCreate(cacheKey.ToLower(), (ICacheEntry entry) => cachedObj2);
        }

        public void RemoveCache(string cacheKey)
        {
            MemoryCache.Remove(cacheKey.ToLower());
        }
    }
}
