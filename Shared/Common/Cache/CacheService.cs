

using Microsoft.Extensions.Caching.Distributed;

namespace Common.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetCacheAsync(string key, string value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime,
                SlidingExpiration = slidingExpireTime
            };

            await _cache.SetStringAsync(key, value, options);
        }

        public async Task<string> GetCacheAsync(string key)
        {
            return await _cache.GetStringAsync(key);
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
