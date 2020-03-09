using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FWTL.Auth.Database;
using FWTL.Auth.Database.Entities;
using FWTL.Common.Credentials;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FWTL.Auth
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IWebHostEnvironment _hostingEnvironment;
        private IContainer _applicationContainer;

        public Startup(IWebHostEnvironment hostingEnvironment)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            _configuration = configuration.Build();

            if (!hostingEnvironment.IsDevelopment())
            {
                _configuration = configuration.Build();
            }

            if (hostingEnvironment.IsDevelopment())
            {
                configuration.AddUserSecrets<Startup>();
                _configuration = configuration.Build();
            }

            _hostingEnvironment = hostingEnvironment;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<AuthDatabaseContext>();
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AuthDatabaseContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients(_configuration))
                .AddOperationalStore(options =>
                {
                    var credentials = new AuthDatabaseCredentials(new SqlServerDatabaseCredentials(_configuration, "Auth"));
                    options.ConfigureDbContext = builder => builder.UseSqlServer(credentials.ConnectionString, sql => sql.MigrationsAssembly(typeof(AuthIdentityServerDatabaseCredentials).Assembly.GetName().Name));
                })
                .AddAspNetIdentity<User>()
                .AddExtensionGrantValidator<ExternalGrantValidator>()
                .AddDeveloperSigningCredential();

            _applicationContainer = IocConfig.RegisterDependencies(services, _hostingEnvironment, _configuration);
            var seedData = _applicationContainer.Resolve<Seed>();
            //var task = Task.Run(async () => await seedData.UpdateAsync());
            task.Wait();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routes => { });
            app.UseIdentityServer();
        }
    }
}