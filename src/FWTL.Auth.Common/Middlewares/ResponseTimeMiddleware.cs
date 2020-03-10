using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace FWTL.Common.Middlewares
{
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly string _source;

        public ResponseTimeMiddleware(RequestDelegate next, ILogger logger, string source)
        {
            _next = next;
            _logger = logger;
            _source = source;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var watch = new Stopwatch();
            watch.Start();
            context.Response.OnStarting(() =>
            {
                watch.Stop();

                string currentUser = context?.User?.Identity?.Name ?? "Anonymous";
                var route = context.GetRouteData();

                if (route != null)
                {
                    var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
                    _logger
                    .Information(
                    "User {currentUser} executed {route} from {source} total elapsed {responseTimeForCompleteRequest} ms",
                    currentUser,
                    $"{context.Request.Method}/{route.Values["controller"]}/{route.Values["action"]}",
                    _source,
                    responseTimeForCompleteRequest);
                }
                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}