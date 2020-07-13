using FWTL.Auth.Database.IdentityServer;
using FWTL.Common.Credentials;
using FWTL.Common.Net.Helpers;
using FWTL.Common.Services;
using FWTL.Core.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FWTL.Auth
{
    public class IocConfig
    {
        public static void OverrideWithLocalCredentials(IServiceCollection services)
        {
        }

        public static void RegisterCredentials(IServiceCollection services)
        {
            services.AddSingleton(b =>
            {
                var configuration = b.GetService<IConfiguration>();
                return new AuthIdentityServerDatabaseCredentials(new SqlServerDatabaseCredentials(configuration, "Auth"));
            });
        }

        public static void RegisterDependencies(IServiceCollection services, IWebHostEnvironment env)
        {
            RegisterCredentials(services);

            if (env.IsDevelopment())
            {
                OverrideWithLocalCredentials(services);
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IProfileService, UserProfileService>();

            services.AddScoped<IGuidService, GuidService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}