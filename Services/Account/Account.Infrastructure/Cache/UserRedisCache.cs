using Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Infrastructure.Cache
{
    public class UserRedisCache : IUserRedisCache
    {
        private readonly ICacheService _cacheService;

        public UserRedisCache(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public Task SetUserDataAsync(string key, string value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            return _cacheService.SetCacheAsync(key, value, absoluteExpireTime, slidingExpireTime);
        }

        public Task<string> GetUserDataAsync(string key)
        {
            return _cacheService.GetCacheAsync(key);
        }

        public Task RemoveUserDataAsync(string key)
        {
            return _cacheService.RemoveCacheAsync(key);
        }
    }
}
