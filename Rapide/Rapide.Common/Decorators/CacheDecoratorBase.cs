using Decor;
using Rapide.Common.Helpers;
using System.Text.Json;

namespace Rapide.Common.Decorators
{
    public class CacheDecoratorBase(CacheService cache) : IDecorator
    {
        protected readonly CacheService cacheService = cache;

        protected virtual void SetCache(string key, object value)
        {
           throw new NotImplementedException();
        }   

        public virtual async Task OnInvoke(Call call)
        {
            var cacheKey = CreateCacheKey(call.Method.Name, call.Arguments);
            var cacheValue = cacheService.TryGetCache<object>(cacheKey);
            if (cacheValue != null)
            {
                call.ReturnValue = cacheValue;
                return;
            }
            await call.Next();
            SetCache(cacheKey, call.ReturnValue);
        }

        public string? CreateCacheKey(string methodName, object[] args)
        {
            var argString = JsonSerializer.Serialize(args);
            return $"{methodName}-{argString}";
        }

    }
}
