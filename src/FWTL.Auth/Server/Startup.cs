using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FWTL.Auth.Database;
using FWTL.Auth.Database.Entities;
using FWTL.Auth.Database.IdentityServer;
using FWTL.Common.Commands;
using FWTL.Common.Credentials;
using FWTL.Common.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NodaTime;
using AutoMapper;

namespace FWTL.Auth.Server
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
            services.AddMvc(configuration =>
            {
                configuration.Filters.Add(new ApiExceptionFilterFactory(_hostingEnvironment.EnvironmentName));
            });
            //.AddJsonOptions(o =>
            //{
            //    o.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            //    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //});

            //var defaultSettings = new JsonSerializerSettings()
            //    .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            //defaultSettings.Converters.Add(new PropertyChangedConverter());
            //JsonConvert.DefaultSettings = () => defaultSettings;

            services.AddAutoMapper(configAction: config => { config.AddProfile(new RequestToCommandProfile(typeof(RequestToCommandProfile))); }, assemblies: typeof(RequestToCommandProfile).Assembly);


            services.AddDbContext<AuthDatabaseContext>();
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AuthDatabaseContext>()
                .AddDefaultTokenProviders();

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
                .AddExtensionGrantValidator<ExternalGrantValidator>()
                .AddDeveloperSigningCredential();

            _applicationContainer = IocConfig.RegisterDependencies(services, _hostingEnvironment, _configuration);

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseRouting();
        }
    }
}