using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FWTL.Auth.Database;
using FWTL.Common.Credentials;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Validation;
using FWTL.Domain.Users;
using FWTL.Rabbitmq;
using FWTL.RabbitMq;
using MassTransit;
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

        public static IContainer RegisterDependencies(IServiceCollection services, IWebHostEnvironment env,
            IConfiguration rootConfiguration)
        {
            var domainAssembly = typeof(RegisterUser).GetTypeInfo().Assembly;

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
                const string format =
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}{Message:lj}{NewLine}{Exception}";
                var configuration = b.Resolve<IConfiguration>();

                return new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(outputTemplate: format)
                    .WriteTo.Seq(configuration["Seq:Url"])
                    .CreateLogger();
            });

            builder.AddMassTransit(x =>
            {
                var commands = typeof(RegisterUser).Assembly.GetTypes().Where(t => typeof(ICommand).IsAssignableFrom(t)).ToList();
                foreach (var command in commands)
                {
                    x.AddConsumers(typeof(CommandConsumer<>).MakeGenericType(command));
                }

                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    //cfg.ConfigureJsonSerializer(config =>
                    //{
                    //    config.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    //    return config;
                    //});

                    //cfg.ConfigureJsonDeserializer(config =>
                    //{
                    //    config.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    //    return config;
                    //});

                    var host = cfg.Host(rootConfiguration["RabbitMq:Url"], h =>
                    {
                        h.Username(rootConfiguration["RabbitMq:Username"]);
                        h.Password(rootConfiguration["RabbitMq:Password"]);
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

            builder.RegisterType<SeedData>().AsSelf();

            builder.RegisterAssemblyTypes(domainAssembly).Where(x => typeof(ICommand).IsAssignableFrom(x) && x.BaseType.Name == "Request").AsSelf();

            builder.RegisterType<RequestDispatcher>().As<ICommandDispatcher>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(domainAssembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<>)).InstancePerLifetimeScope();

            builder.RegisterType<EventFactory>().As<IEventFactory>().InstancePerLifetimeScope();
            builder.RegisterType<EventDispatcher>().As<IEventDispatcher>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(domainAssembly).AsClosedTypesOf(typeof(AppAbstractValidation<>)).InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}