using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Cache
{
    public interface ICacheService
    {
        Task SetCacheAsync(string key, string value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
        Task<string> GetCacheAsync(string key);
        Task RemoveCacheAsync(string key);
    }
}
