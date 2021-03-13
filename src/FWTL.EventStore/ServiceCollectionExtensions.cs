using System;
using EventStore.Client;
using FWTL.Core.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.EventStore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventStore(this IServiceCollection services,Uri address)
        {
            services.AddSingleton(b =>
            {
                var settings = new EventStoreClientSettings
                {
                    ConnectivitySettings = {
                        Address = address
                    }
                };

                return new EventStoreClient(settings);
            });
            services.AddScoped<IAggregateStore, EventStoreAggregateStore>();
        }
    }
}
