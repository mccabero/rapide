using Rapide.Common.Helpers;

namespace Rapide.Common.Decorators
{
    public class PermanentCacheDecorator(CacheService cache) : CacheDecoratorBase(cache)
    {
        protected override void SetCache(string key, object value)
        {
            cacheService.SetPermanentCache(key, value);
        }
    }
}
