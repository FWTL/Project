using System;
using System.Net.Http;
using System.Reflection;
using FluentValidation;
using FWTL.Common.Cqrs;
using FWTL.Common.Cqrs.Mappers;
using FWTL.Common.Services;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Core.Specification;
using FWTL.CurrentUser;
using FWTL.Domain.Users;
using FWTL.EventStore;
using FWTL.RabbitMq;
using FWTL.TelegramClient;
using FWTL.TimeZones;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Polly;
using Polly.Extensions.Http;

namespace FWTL.Management
{
    public static class IocConfig
    {
        public static ServiceProvider RegisterDependencies(IServiceCollection services, IWebHostEnvironment env)
        {
            var domainAssembly = typeof(GetMe).GetTypeInfo().Assembly;


            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces().WithTransientLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces().WithScopedLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces().WithScopedLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(ISpecificationFor<,>)))
                    .AsImplementedInterfaces().WithScopedLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(filter => filter.Where(implementation => typeof(ICommand).IsAssignableFrom(implementation) && typeof(IRequest).IsAssignableFrom(implementation)))
                    .AsSelf()
                    .WithScopedLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(filter => filter.Where(implementation => typeof(IQuery).IsAssignableFrom(implementation) && typeof(IRequest).IsAssignableFrom(implementation)))
                    .AsSelf()
                    .WithScopedLifetime()
            );

          
            services.AddScoped<IEventFactory, EventFactory>();
            services.AddScoped<ICommandDispatcher, RequestDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            services.AddSingleton<IGuidService, GuidService>();
            services.AddCurrentUserService();
            services.AddSingleton<IClock>(b => SystemClock.Instance);
            services.AddScoped<RequestToCommandMapper>();
            services.AddScoped<RequestToQueryMapper>();

            services.AddSingleton<IExceptionHandler, ExceptionHandler>();

          
            services.AddScoped<IEventFactory, EventFactory>();

            return services.BuildServiceProvider();
        }

       
    }
}