using System;
using System.Threading.Tasks;
using EventStore.Client;
using Polly;

namespace FWTL.EventStore
{
    internal static class Policies
    {
        internal static AsyncPolicy SqRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
        internal static AsyncPolicy EventStoreRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
        internal static AsyncPolicy<bool> RedisFallbackPolicy = Policy<bool>.Handle<Exception>().FallbackAsync(x => Task.FromResult(true));
        internal static AsyncPolicy PublishRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
    }
}
