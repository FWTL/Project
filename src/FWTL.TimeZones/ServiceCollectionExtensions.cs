using FWTL.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.TimeZones
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTimeZonesService(this IServiceCollection services)
        {
            services.AddSingleton<ITimeZonesService, TimeZonesService>();
        }
    }
}