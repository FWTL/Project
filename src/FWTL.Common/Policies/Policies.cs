using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace FWTL.Common.Policies
{
    public static class Policies
    {
        public static IAsyncPolicy<HttpResponseMessage> Retry(int retryAttempts)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public static IAsyncPolicy<HttpResponseMessage> Timeout(int seconds = 2) =>
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds));
    }
}
