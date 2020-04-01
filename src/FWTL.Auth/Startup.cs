using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FWTL.Auth.Database;
using FWTL.Auth.Database.IdentityServer;
using FWTL.Common.Commands;
using FWTL.Common.Credentials;
using FWTL.Common.Net.Filters;
using FWTL.Core.Commands;
using FWTL.Domain.Users;
using FWTL.RabbitMq;
using IdentityServer4.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Serilog;
using ILogger = Serilog.ILogger;

namespace FWTL.Auth
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
            services.AddCors();

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

            const string format =
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}{Message:lj}{NewLine}{Exception}";

            services.AddSingleton<ILogger>(b => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: format)
                .WriteTo.Seq(_configuration["Seq:Url"])
                .CreateLogger());

            services.AddAutoMapper(
                config => { config.AddProfile(new RequestToCommandProfile(typeof(RegisterUser))); },
                typeof(RequestToCommandProfile).Assembly);

            services.AddDbContext<AuthDatabaseContext>();

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
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
                .AddDeveloperSigningCredential();

            var cors = new DefaultCorsPolicyService(new LoggerFactory().CreateLogger<DefaultCorsPolicyService>())
            {
                AllowAll = true
            };
            services.AddSingleton<ICorsPolicyService>(cors);

            IocConfig.RegisterDependencies(services, _hostingEnvironment);

            services.AddMassTransit(x =>
            {
                var commands = typeof(RegisterUser).Assembly.GetTypes().Where(t => typeof(ICommand).IsAssignableFrom(t))
                    .ToList();

                x.AddConsumers(typeof(CommandConsumer<RegisterUser.Command>));

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
                        ec.ConfigureConsumer(context, typeof(CommandConsumer<RegisterUser.Command>));
                    });
                }));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "FWTL.Auth", Version = "v1" });
                c.CustomSchemaIds(x =>
                {
                    int plusIndex = x.FullName.IndexOf("+");
                    int lastIndexOfDot = x.FullName.LastIndexOf(".");
                    int length = 0;

                    if (plusIndex != -1)
                    {
                        length = plusIndex - lastIndexOfDot - 1;
                    }
                    else
                    {
                        length = x.FullName.Length - lastIndexOfDot - 1;
                    }

                    return x.FullName.Substring(lastIndexOfDot + 1, length);
                });
            });
        }

        public void Configure(IApplicationBuilder app, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            app.UseCors(policy =>
            {
                policy = policy.AllowAnyOrigin();
                policy = policy.AllowAnyMethod();
                policy = policy.AllowAnyHeader();
            });

            app.UseIdentityServer();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FWTL.Auth");
                c.DisplayRequestDuration();
            });

            var serviceProvider = app.ApplicationServices;

            Task startBusTask = Task.Run(async () => await ConfigureAsync(app));
            startBusTask.Wait();

            Task seedDatabase = Task.Run(async () => await new SeedData(userManager, roleManager).UpdateAsync());
            seedDatabase.Wait();
        }

        public async Task ConfigureAsync(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var bus = serviceProvider.GetService<IBusControl>();

            await bus.StartAsync();
        }
    }
}