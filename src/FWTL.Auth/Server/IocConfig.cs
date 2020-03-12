﻿using System.Reflection;
using FWTL.Auth.Database;
using FWTL.Common.Credentials;
using FWTL.Common.Services;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.Domain.Users;
using FWTL.RabbitMq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Serilog;

namespace FWTL.Auth.Server
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
                return new AuthDatabaseCredentials(new SqlServerDatabaseCredentials(configuration, "Auth"));
            });
        }

        public static void RegisterDependencies(IServiceCollection services, IWebHostEnvironment env)
        {
            var domainAssembly = typeof(RegisterUser).GetTypeInfo().Assembly;

            RegisterCredentials(services);

            if (env.IsDevelopment())
            {
                OverrideWithLocalCredentials(services);
            }

            services.AddSingleton<ILogger>(b =>
            {
                const string format =
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}{Message:lj}{NewLine}{Exception}";
                var configuration = b.GetService<IConfiguration>();

                return new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(outputTemplate: format)
                    .WriteTo.Seq(configuration["Seq:Url"])
                    .CreateLogger();
            });

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(AppAbstractValidation<>)))
                    .AsSelf().WithScopedLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandlerAsync<>)))
                    .AsImplementedInterfaces().WithScopedLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(filter => filter.Where(implementation => typeof(ICommand).IsAssignableFrom(implementation) && typeof(IRequest).IsAssignableFrom(implementation)))
                    .AsSelf()
                    .WithScopedLifetime()
            );

            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IEventFactory, EventFactory>();
            services.AddScoped<ICommandDispatcher, RequestDispatcher>();
            services.AddScoped<ICommandDispatcher, RequestDispatcher>();
            services.AddScoped<IGuidService, GuidService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IClock>(b => SystemClock.Instance);
            services.AddScoped<IRequestToCommandMapper, RequestToCommandMapper>();
        }
    }
}