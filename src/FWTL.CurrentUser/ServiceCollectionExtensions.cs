using FWTL.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.CurrentUser
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCurrentUserService(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}