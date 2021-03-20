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
            

            return services.BuildServiceProvider();
        }

       
    }
}