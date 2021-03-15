using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FWTL.CurrentUser
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
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
    }
}
