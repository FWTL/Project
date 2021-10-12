using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;
using Polly.Wrap;

namespace FWTL.Common.Policies
{
    public static class Policies
    {
        public static IAsyncPolicy<HttpResponseMessage> Retry(int retryAttempts)
        {
            Polly.Retry.AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            Polly.Fallback.AsyncFallbackPolicy<HttpResponseMessage> fallbackPolicy = HttpPolicyExtensions.HandleTransientHttpError().FallbackAsync(fallbackAction: (result, context, token) =>
                {
                    var msg = result.Exception?.Message ?? result.Result.StatusCode.ToString();
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                    {
                        Content = new StringContent(msg)
                    });
                },
            (result, context) => Task.CompletedTask);

            return Policy.WrapAsync(retryPolicy, fallbackPolicy);
        }

        public static IAsyncPolicy<HttpResponseMessage> Timeout(int seconds = 2) =>
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds));
    }
}
