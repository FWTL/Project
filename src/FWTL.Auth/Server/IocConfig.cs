using Autofac;
using Autofac.Extensions.DependencyInjection;
using FWTL.Auth.Database;
using FWTL.Common.Credentials;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace FWTL.Auth.Server
{
    public class IocConfig
    {
        public static void OverrideWithLocalCredentials(ContainerBuilder builder)
        {
        }

        public static void RegisterCredentials(ContainerBuilder builder)
        {
            builder.Register(b =>
            {
                var configuration = b.Resolve<IConfiguration>();
                return new AuthDatabaseCredentials(new SqlServerDatabaseCredentials(configuration, "Auth"));
            }).SingleInstance();
        }

        public static IContainer RegisterDependencies(IServiceCollection services, IWebHostEnvironment env, IConfiguration rootConfiguration)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            RegisterCredentials(builder);

            if (env.IsDevelopment())
            {
                OverrideWithLocalCredentials(builder);
            }

            builder.Register(b => rootConfiguration).SingleInstance();

            builder.Register<ILogger>(b =>
            {
                const string format = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}{Message:lj}{NewLine}{Exception}";
                var configuration = b.Resolve<IConfiguration>();

                return new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(outputTemplate: format)
                    .WriteTo.Seq(configuration["Seq:Url"])
                    .CreateLogger();
            });

            builder.RegisterType<SeedData>().AsSelf();

            return builder.Build();
        }
    }
}