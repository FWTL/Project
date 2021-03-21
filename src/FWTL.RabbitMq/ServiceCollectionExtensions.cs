using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using FWTL.Common.Cqrs;
using FWTL.Common.Cqrs.Mappers;
using FWTL.Common.Setup.Credentials;
using FWTL.Common.Setup.Profiles;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using FWTL.Core.Queries;
using FWTL.Core.Specification;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace FWTL.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRabbitMq<TLookupType>(this IServiceCollection services, RabbitMqCredentials credentials, RedisCredentials redisCredentials)
        {
            services.AddAutoMapper(
            config =>
            {
                config.AddProfile(new RequestToCommandProfile(typeof(TLookupType)));
                config.AddProfile(new RequestToQueryProfile(typeof(TLookupType)));
            }, typeof(TLookupType).Assembly);

            var domainAssembly = typeof(TLookupType).GetTypeInfo().Assembly;

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

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(filter => filter.Where(implementation => typeof(IQuery).IsAssignableFrom(implementation) && typeof(IRequest).IsAssignableFrom(implementation)))
                    .AsSelf()
                    .WithScopedLifetime()
            );

            services.Scan(scan =>
                scan.FromAssemblies(domainAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IAggregateMap<>)))
                    .AsImplementedInterfaces().WithScopedLifetime()
            );

            services.AddSingleton<IExceptionHandler, ExceptionHandler>();
            services.AddScoped<IEventFactory, EventFactory>();
            services.AddScoped<ICommandDispatcher, RequestDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();

            services.AddScoped<RequestToCommandMapper>();
            services.AddScoped<RequestToQueryMapper>();



            services.AddMassTransit(x =>
            {
                x.SetRedisSagaRepositoryProvider(r => r.DatabaseConfiguration(redisCredentials.ConnectionString));
                x.AddSagaStateMachines(typeof(TLookupType).Assembly);

                var commands = typeof(TLookupType).Assembly.GetTypes()
                    .Where(t => t.IsNested && t.Name == "Handler")
                    .Select(t => t.GetInterfaces().First())
                    .Where(t => typeof(ICommandHandler<>).IsAssignableFrom(t.GetGenericTypeDefinition()))
                    .ToList();

                foreach (var commandType in commands)
                {
                    var typeArguments = commandType.GetGenericArguments();
                    x.AddConsumer(typeof(CommandConsumer<>).MakeGenericType(typeArguments));
                }

                var queries = typeof(TLookupType).Assembly.GetTypes()
                    .Where(t => t.IsNested && t.Name == "Handler")
                    .Select(t => t.GetInterfaces().First())
                    .Where(t => typeof(IQueryHandler<,>).IsAssignableFrom(t.GetGenericTypeDefinition()))
                    .ToList();

                foreach (var queryType in queries)
                {
                    var typeArguments = queryType.GetGenericArguments();
                    x.AddConsumer(typeof(QueryConsumer<,>).MakeGenericType(typeArguments));
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

                    cfg.Host(credentials.Url, h =>
                    {
                        h.Username(credentials.UserName);
                        h.Password(credentials.Password);
                    });

                    cfg.ReceiveEndpoint("commands", ec =>
                    {
                        foreach (var commandType in commands)
                        {
                            var typeArguments = commandType.GetGenericArguments();
                            ec.ConfigureConsumer(context, typeof(CommandConsumer<>).MakeGenericType(typeArguments));
                        }
                        ec.ConfigureSagas(context);
                    });

                    cfg.ReceiveEndpoint("queries", ec =>
                    {
                        foreach (var queryType in queries)
                        {
                            var typeArguments = queryType.GetGenericArguments();
                            ec.ConfigureConsumer(context, typeof(QueryConsumer<,>).MakeGenericType(typeArguments));
                        }
                    });

                    cfg.UseMessageScheduler(new Uri("queue:hangfire"));
                }));
            });
        }
    }
}
