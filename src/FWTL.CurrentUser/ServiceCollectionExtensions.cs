using FWTL.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.CurrentUser
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCurrentUserService(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}
