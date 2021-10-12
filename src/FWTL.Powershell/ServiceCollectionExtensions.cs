using FWTL.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.Powershell
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLocalInfrastructureSetupService(this IServiceCollection services)
        {
            services.AddScoped<IInfrastructureService, LocalInfrastructureService>();
        }
    }
}