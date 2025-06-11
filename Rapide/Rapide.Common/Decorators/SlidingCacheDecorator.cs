using Rapide.Common.Helpers;

namespace Rapide.Common.Decorators
{
    public class SlidingCacheDecorator(CacheService cache) : CacheDecoratorBase(cache)
    {
        protected override void SetCache(string key, object value)
        {
            cacheService.SetSlidingCache(key, value);
        }
    }
}
