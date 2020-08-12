using FWTL.Aggragate;
using FWTL.Auth.Database;
using FWTL.Auth.Database.IdentityServer;
using FWTL.Common.Credentials;
using FWTL.Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace FWTL.Auth
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IWebHostEnvironment hostingEnvironment)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            _configuration = configuration.Build();

            if (hostingEnvironment.IsDevelopment())
            {
                configuration.AddUserSecrets<Startup>(true);
                _configuration = configuration.Build();
            }

            _hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("localhost").AllowAnyHeader().AllowAnyMethod();
                    });
            });

            const string format =
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}{Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: format)
                .Enrich.FromLogContext()
                .WriteTo.Seq(_configuration["Seq:Url"])
                .CreateLogger();

            services.AddDbContext<AuthDatabaseContext>();
            services.AddIdentity<User, Role>().AddEntityFrameworkStores<AuthDatabaseContext>();

            services.AddIdentityServer()
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients(_configuration))
                .AddOperationalStore(options =>
                {
                    var credentials =
                        new AuthDatabaseCredentials(new SqlServerDatabaseCredentials(_configuration, "Auth"));
                    options.ConfigureDbContext = builder => builder.UseSqlServer(credentials.ConnectionString,
                        sql => sql.MigrationsAssembly(typeof(AuthIdentityServerDatabaseCredentials).Assembly.GetName()
                            .Name));
                })
                .AddAspNetIdentity<User>()
                .AddDeveloperSigningCredential();

            IocConfig.RegisterDependencies(services, _hostingEnvironment);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors();

            app.UseIdentityServer();
            app.UseRouting();
        }
    }
}