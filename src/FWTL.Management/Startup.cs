using AutoMapper;
using FWTL.Aggregate;
using FWTL.Auth.Database;
using FWTL.Common.Commands;
using FWTL.Common.Net.Filters;
using FWTL.Common.Queries;
using FWTL.Core.Commands;
using FWTL.Core.Queries;
using FWTL.Domain.Users;
using FWTL.RabbitMq;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Serilog;
using Serilog.Events;
using System.Linq;
using System.Threading.Tasks;

namespace FWTL.Management
{
    ////dotnet swagger tofile --output api.json C:\Projects\FWTLAuth\src\FWTL.Api\bin\Debug\netcoreapp3.1\FWTL.Api.dll v1

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

            if (hostingEnvironment.IsDevelopment())
            {
                configuration.AddUserSecrets<Startup>(true);
                _configuration = configuration.Build();
            }

            _hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
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

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: format)
                .Enrich.FromLogContext()
                    .WriteTo.Seq(_configuration["Seq:Url"])
                .CreateLogger();

            services.AddAutoMapper(
                config =>
                {
                    config.AddProfile(new RequestToCommandProfile(typeof(RegisterUser)));
                    config.AddProfile(new RequestToQueryProfile(typeof(RegisterUser)));
                }, typeof(RequestToCommandProfile).Assembly);

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

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.Audience = "api";
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(x =>
            {
                x.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(new[] { JwtBearerDefaults.AuthenticationScheme })
                    .RequireAuthenticatedUser().Build();
            });

            IocConfig.RegisterDependencies(services, _hostingEnvironment);

            services.AddMassTransit(x =>
            {
                var commands = typeof(RegisterUser).Assembly.GetTypes().Where(t => typeof(ICommand).IsAssignableFrom(t))
                    .ToList();

                foreach (var commandType in commands)
                {
                    x.AddConsumer(typeof(CommandConsumer<>).MakeGenericType(commandType));
                }

                var queries = typeof(RegisterUser).Assembly.GetTypes()
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

                    var host = cfg.Host(_configuration["RabbitMq:Url"], h =>
                    {
                        h.Username(_configuration["RabbitMq:Username"]);
                        h.Password(_configuration["RabbitMq:Password"]);
                    });

                    cfg.ReceiveEndpoint("commands", ec =>
                    {
                        foreach (var commandType in commands)
                        {
                            ec.ConfigureConsumer(context, typeof(CommandConsumer<>).MakeGenericType(commandType));
                        }
                    });

                    cfg.ReceiveEndpoint("queries", ec =>
                    {
                        foreach (var queryType in queries)
                        {
                            var typeArguments = queryType.GetGenericArguments();
                            ec.ConfigureConsumer(context, typeof(QueryConsumer<,>).MakeGenericType(typeArguments));
                        }
                    });
                }));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "FWTL.Api", Version = "v1" });
                c.CustomSchemaIds(x =>
                {
                    var fragments = x.FullName.Split("+");
                    var dotFragments = fragments[0].Split(".");
                    if (fragments.Length <= 2)
                    {
                        return dotFragments.Last();
                    }

                    var leftPart = dotFragments.Last();
                    var rightPart = string.Join(".", fragments.Where((f, index) => index > 1));
                    return leftPart + "." + rightPart;
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, UserManager<User> userManager, RoleManager<Role> roleManager)
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
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FWTL.Api");
                c.DisplayRequestDuration();
            });

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