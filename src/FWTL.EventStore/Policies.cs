using System;
using System.Threading.Tasks;
using EventStore.Client;
using Polly;
using StackExchange.Redis;

namespace FWTL.EventStore
{
    internal static class Policies
    {
        internal static AsyncPolicy SqRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
        internal static AsyncPolicy EventStoreRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
        internal static AsyncPolicy<bool> RedisFallbackPolicy = Policy<bool>.Handle<Exception>().FallbackAsync(x => Task.FromResult(true));
        internal static AsyncPolicy<RedisValue> RedisValueFallbackPolicy = Policy<RedisValue>.Handle<Exception>().FallbackAsync(x => Task.FromResult(RedisValue.Null));
        internal static AsyncPolicy PublishRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
    }
}
