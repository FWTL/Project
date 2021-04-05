using System;
using System.Threading.Tasks;
using EventStore.Client;
using Polly;

namespace FWTL.EventStore
{
    public static class Policies
    {
        public static AsyncPolicy SqRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
        public static AsyncPolicy EventStoreRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
        public static AsyncPolicy<bool> RedisFallbackPolicy = Policy<bool>.Handle<Exception>().FallbackAsync(x => Task.FromResult(true));
    }
}
