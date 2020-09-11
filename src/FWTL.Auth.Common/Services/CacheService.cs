using FWTL.Core.Services;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FWTL.Common.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;

        public CacheService(IDatabase cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan? expire) where T : class
        {
            string json = JsonSerializer.Serialize(value);
            _cache.StringSetAsync(key, json, expire);
        }

        public T Get<T>(string key, T value) where T : class
        {
            var json = _cache.StringGet(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> fallback, bool isForced, TimeSpan? expire) where T : class
        {
            if (isForced)
            {
                var result = await fallback();
                Set<T>(key, result, expire);
                return result;
            }

            RedisValueWithExpiry redisValue = _cache.StringGetWithExpiry(key);
            if (string.IsNullOrWhiteSpace(redisValue.Value))
            {
                var result = await fallback();
                Set<T>(key, result, expire);
                return result;
            }
            else if(expire.HasValue && redisValue.Expiry?.TotalSeconds < expire.Value.TotalSeconds / 2)
            {
                _cache.KeyExpire(key, expire);
            }

            return JsonSerializer.Deserialize<T>(redisValue.Value);
        }
    }
}