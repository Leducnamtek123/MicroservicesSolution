using System;
using System.Threading.Tasks;

namespace Account.Infrastructure.Cache
{
    public interface IUserRedisCache
    {
        Task SetUserDataAsync(string key, string value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
        Task<string> GetUserDataAsync(string key);
        Task RemoveUserDataAsync(string key);
    }
}
