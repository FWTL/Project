using System.Threading.Tasks;
using FWTL.Common.Policies;
using FWTL.Common.Services;
using FWTL.Core.Services;
using FWTL.CurrentUser;
using FWTL.Database.Access;
using FWTL.Domain.Users;
using FWTL.EventStore;
using FWTL.Hangfire;
using FWTL.Management.Configuration;
using FWTL.Management.Filters;
using FWTL.Powershell;
using FWTL.RabbitMq;
using FWTL.Redis;
using FWTL.Serilog;
using FWTL.Swagger;
using FWTL.TelegramClient;
using FWTL.TimeZones;
using Hangfire;
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

            services.AddControllers(configuration =>
            {
                configuration.Filters.Add<ApiExceptionAttribute>();
            });

            Log.Logger = Log.Logger.AddSerilog().AddSeq(_solutionConfiguration.SeqUrl).CreateLogger();

            services.AddDatabase<AppDatabaseContext>(_solutionConfiguration.AppDatabaseCredentials.ConnectionString);

            services.AddTelegramClient(_solutionConfiguration.TelegramUrl)
                .AddPolicyHandler(Policies.Retry(3))
                .AddPolicyHandler(Policies.Timeout(30));

            services.AddTimeZonesService();

            services.AddEventStore(_solutionConfiguration.EventStoreUrl);

            services.AddHangfire(_solutionConfiguration.HangfireDatabaseCredentials.ConnectionString);

            services.AddRedis(_solutionConfiguration.RedisCredentials.ConnectionString);

            services.AddRabbitMq<GetMe>(_solutionConfiguration.RabbitMqCredentials, _solutionConfiguration.RedisCredentials);

            services.AddSwagger("FWTL.API");

            services.AddCurrentUserService();

            services.AddSingleton<IClock>(b => SystemClock.Instance);

            services.AddSingleton<IGuidService, GuidService>();
            services.AddLocalInfrastructureSetupService();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();

            app.UseRouting();

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