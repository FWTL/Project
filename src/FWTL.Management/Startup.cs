using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FWTL.Common.Setup.Extensions;
using FWTL.Common.Setup.Profiles;
using FWTL.Core.Aggregates;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.CurrentUser;
using FWTL.Database.Access;
using FWTL.Domain.Accounts;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Domain.Accounts.Maps;
using FWTL.Domain.Users;
using FWTL.Management.Configuration;
using FWTL.Management.Filters;
using FWTL.RabbitMq;
using FWTL.TimeZones;
using Hangfire;
using Hangfire.SqlServer;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Serialization.SystemTextJson;
using Serilog;
using Serilog.Events;

namespace FWTL.Management
{
    ////dotnet swagger tofile --output api.json C:\Projects\FWTLAuth\src\FWTL.Api\bin\Debug\netcoreapp3.1\FWTL.Api.dll v1

    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SolutionConfiguration _solutionConfiguration;

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
            _solutionConfiguration = new SolutionConfiguration(_configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("localhost").AllowAnyHeader().AllowAnyMethod();
                    });
            });

            services.AddControllers(configuration =>
            {
                configuration.Filters.Add(new ApiExceptionFilterFactory(_hostingEnvironment.EnvironmentName));
            });

            //var defaultSettings = new JsonSerializerSettings()
            //    .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            //JsonConvert.DefaultSettings = () => defaultSettings;

            const string format =
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}{Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: format)
                .Enrich.FromLogContext()
                    .WriteTo.Seq(_configuration.GetNotNullOrEmpty("Seq:Url"))
                .CreateLogger();

            services.AddAutoMapper(
                config =>
                {
                    config.AddProfile(new RequestToCommandProfile(typeof(GetMe)));
                    config.AddProfile(new RequestToQueryProfile(typeof(GetMe)));
                }, typeof(RequestToCommandProfile).Assembly);

            services.AddDbContext<AppDatabaseContext>();


            ServiceProvider servicesProvider = IocConfig.RegisterDependencies(services, _hostingEnvironment);

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(servicesProvider.GetService<HangfireDatabaseCredentials>().ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();

            services.AddScoped<IAggregateMap<AccountAggregate>, MapToAccounts>();

            services.AddRedis("localhost,abortConnect=False,allowAdmin=true");

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<AccountSetupSaga, AccountSetupState>()
                .RedisRepository(_solutionConfiguration.RedisCredentials.ConnectionString);

                var commands = typeof(GetMe).Assembly.GetTypes()
                    .Where(t => t.IsNested && t.Name == "Handler")
                    .Select(t => t.GetInterfaces().First())
                    .Where(t => typeof(ICommandHandler<>).IsAssignableFrom(t.GetGenericTypeDefinition()))
                    .ToList();

                foreach (var commandType in commands)
                {
                    var typeArguments = commandType.GetGenericArguments();
                    x.AddConsumer(typeof(CommandConsumer<>).MakeGenericType(typeArguments));
                }

                var queries = typeof(GetMe).Assembly.GetTypes()
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

                    cfg.Host(_configuration.GetNotNullOrEmpty("RabbitMq:Url"), h =>
                    {
                        h.Username(_configuration.GetNotNullOrEmpty("RabbitMq:Username"));
                        h.Password(_configuration.GetNotNullOrEmpty("RabbitMq:Password"));
                    });

                    cfg.ReceiveEndpoint("commands", ec =>
                    {
                        foreach (var commandType in commands)
                        {
                            var typeArguments = commandType.GetGenericArguments();
                            ec.ConfigureConsumer(context, typeof(CommandConsumer<>).MakeGenericType(typeArguments));
                        }
                        ec.ConfigureSaga<AccountSetupState>(context);
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

            services.AddSwagger();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();

            app.UseCors(policy =>
            {
                policy = policy.AllowAnyOrigin();
                policy = policy.AllowAnyMethod();
                policy = policy.AllowAnyHeader();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FWTL.Api");
                c.DisplayRequestDuration();
            });

            Task startBusTask = Task.Run(async () => await ConfigureAsync(app));
            startBusTask.Wait();
        }

        public async Task ConfigureAsync(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var bus = serviceProvider.GetService<IBusControl>();

            await bus.StartAsync();
        }
    }
}