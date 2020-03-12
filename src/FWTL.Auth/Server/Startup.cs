﻿using System.Linq;
using AutoMapper;
using FWTL.Auth.Database;
using FWTL.Auth.Database.Entities;
using FWTL.Auth.Database.IdentityServer;
using FWTL.Common.Commands;
using FWTL.Common.Credentials;
using FWTL.Common.Net.Filters;
using FWTL.Core.Commands;
using FWTL.Domain.Users;
using FWTL.RabbitMq;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace FWTL.Auth.Server
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(configuration =>
            {
                configuration.Filters.Add(new ApiExceptionFilterFactory(_hostingEnvironment.EnvironmentName));
            });
            //.AddJsonOptions(o =>
            //{
            //    o.JsonSerializerOptions.Con;
            //    o.JsonSerializerOptions.IgnoreNullValues = true;
            //});

            var defaultSettings = new JsonSerializerSettings()
                .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            JsonConvert.DefaultSettings = () => defaultSettings;

            services.AddAutoMapper(
                config => { config.AddProfile(new RequestToCommandProfile(typeof(RequestToCommandProfile))); },
                typeof(RequestToCommandProfile).Assembly);

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

            IocConfig.RegisterDependencies(services, _hostingEnvironment);

            services.AddMassTransit(x =>
            {
                var commands = typeof(RegisterUser).Assembly.GetTypes().Where(t => typeof(ICommand).IsAssignableFrom(t))
                    .ToList();

                foreach (var command in commands)
                {
                    x.AddConsumers(typeof(CommandConsumer<>).MakeGenericType(command));
                }

                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.ConfigureJsonSerializer(config =>
                    {
                        config.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                        return config;
                    });

                    cfg.ConfigureJsonDeserializer(config =>
                    {
                        config.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                        return config;
                    });

                    var host = cfg.Host(_configuration["RabbitMq:Url"], h =>
                    {
                        h.Username(_configuration["RabbitMq:Username"]);
                        h.Password(_configuration["RabbitMq:Password"]);
                    });

                    cfg.ReceiveEndpoint("commands", ec =>
                    {
                        foreach (var command in commands)
                        {
                            ec.ConfigureConsumer(context, typeof(CommandConsumer<>).MakeGenericType(command));
                        }
                    });
                }));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}