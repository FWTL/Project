using System.Reflection;
using FluentValidation;
using FWTL.Common.Cqrs;
using FWTL.Common.Cqrs.Mappers;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Helpers;
using FWTL.Core.Queries;
using FWTL.Core.Services;
using FWTL.Core.Specification;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.Database.Access
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCqrs<TLookupType>(this IServiceCollection services)
        {
           


           
           
            services.AddScoped<RequestToCommandMapper>();
            services.AddScoped<RequestToQueryMapper>();

            
        }
    }
}
