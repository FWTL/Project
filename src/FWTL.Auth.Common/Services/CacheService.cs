﻿using FWTL.Core.Services;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FWTL.Common.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;
        private static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        static CacheService()
        {
            SerializeOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        }

        public CacheService(IDatabase cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan? expire) where T : class
        {
            string json = JsonSerializer.Serialize(value, SerializeOptions);
            _cache.StringSetAsync(key, json, expire);
        }

        public T Get<T>(string key, T value) where T : class
        {
            var json = _cache.StringGet(key);
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<T>(json);
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

            if (expire.HasValue && redisValue.Expiry?.TotalSeconds < expire.Value.TotalSeconds / 2)
            {
                _cache.KeyExpire(key, expire);
            }

            return JsonSerializer.Deserialize<T>(redisValue.Value, SerializeOptions);
        }
    }
}