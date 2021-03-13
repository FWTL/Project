using FWTL.Core.Services;
using FWTL.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FWTL.TimeZones
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRedis(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(b => ConnectionMultiplexer.Connect(connectionString));

            services.AddSingleton(b =>
            {
                var redis = b.GetService<ConnectionMultiplexer>();
                return redis.GetDatabase();
            });

            services.AddScoped<ICacheService, RedisCacheService>();
        }
    }
}
