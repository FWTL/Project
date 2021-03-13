using EventStore.Client;
using FluentValidation;
using FWTL.Common.Credentials;
using FWTL.Common.Helpers;
using FWTL.Common.Services;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Database;
using FWTL.Domain.Users;
using FWTL.EventStore;
using FWTL.RabbitMq;
using FWTL.TelegramClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;
using System;
using System.Net.Http;
using System.Reflection;
using FWTL.Core.Specification;
using FWTL.Database.Access;

namespace FWTL.Management
{
    public static class IocConfig
    {
        public static void OverrideWithLocalCredentials(IServiceCollection services)
        {
            services.AddSingleton(b =>
            {
                var configuration = b.GetService<IConfiguration>();
                return new RedisCredentials(new RedisLocalCredentialsBase(configuration));
            });
        }

        public static void RegisterCredentials(IServiceCollection services)
        {
            services.AddSingleton(b =>
            {
                var configuration = b.GetService<IConfiguration>();
                return new AppDatabaseCredentials(new SqlServerDatabaseCredentials(configuration, "App"));
            });

            services.AddSingleton(b =>
            {
                var configuration = b.GetService<IConfiguration>();
                return new HangfireDatabaseCredentials(new SqlServerDatabaseCredentials(configuration, "Hangfire"));
            });

            services.AddSingleton(b =>
            {
                var configuration = b.GetService<IConfiguration>();
                return new EventStoreCredentials(new EventStoreCredentialsBase(configuration));
            });
        }

        public static ServiceProvider RegisterDependencies(IServiceCollection services, IWebHostEnvironment env)
        {
            var domainAssembly = typeof(GetMe).GetTypeInfo().Assembly;

            RegisterCredentials(services);

            if (env.IsDevelopment())
            {
                OverrideWithLocalCredentials(services);
            }

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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IEventFactory, EventFactory>();
            services.AddScoped<ICommandDispatcher, RequestDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            services.AddSingleton<IGuidService, GuidService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IClock>(b => SystemClock.Instance);
            services.AddScoped<RequestToCommandMapper>();
            services.AddScoped<RequestToQueryMapper>();
            services.AddSingleton<ITimeZonesService, TimeZonesService>();
            services.AddSingleton<IExceptionHandler, ExceptionHandler>();

            services.AddHttpClient<ITelegramClient, Client>((service, client) =>
            {
                var configuration = service.GetService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Telegram:Url"]);
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(TimeoutPolicy(30));

            services.AddScoped<IDatabaseContext, AppDatabaseContext>();

            services.AddSingleton(b =>
            {
                var credentials = b.GetService<RedisCredentials>();
                return ConnectionMultiplexer.Connect(credentials.ConnectionString);
            });

            services.AddSingleton(b =>
            {
                var redis = b.GetService<ConnectionMultiplexer>();
                return redis.GetDatabase();
            });

            services.AddSingleton(b =>
            {
                var configuration = b.GetService<IConfiguration>();
                var redis = b.GetService<ConnectionMultiplexer>();
                return redis.GetServer(host: configuration["Redis:Url"], port: 6379);
            });

            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IAggregateStore, EventStoreAggregateStore>();

            services.AddSingleton(b =>
            {
                var settings = new EventStoreClientSettings
                {
                    ConnectivitySettings = {
                        Address = new Uri("http://localhost:2113")
                    }
                };

                return new EventStoreClient(settings);
            });

            services.AddScoped<IEventFactory, EventFactory>();

            return services.BuildServiceProvider();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy(int seconds = 2) =>
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds));
    }
}