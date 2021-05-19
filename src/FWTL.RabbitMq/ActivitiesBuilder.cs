using System;
using System.Collections.Generic;
using FWTL.Core.Commands;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;

namespace FWTL.RabbitMq
{
    public class ActivitiesBuilder
    {
        private readonly List<Type> _types = new List<Type>();

        public ActivitiesBuilder()
        {
        }

        public void Add<TCommand>() where TCommand : ICommand
        {
            _types.Add(typeof(TCommand));
        }

        public void AddExecuteActivity(IServiceCollectionBusConfigurator serviceCollectionBusConfigurator)
        {
            foreach (var commandType in _types)
            {
                Type commandActivityType = typeof(CommandActivity<>).MakeGenericType(commandType);
                serviceCollectionBusConfigurator.AddExecuteActivity(commandActivityType, null);
            }
        }

        public void ConfigureExecuteActivity(IBusRegistrationContext busRegistrationContext, IRabbitMqBusFactoryConfigurator rabbitMqBusFactoryConfigurator)
        {
            foreach (var commandType in _types)
            {
                string endpointName = commandType.FullName.Replace(".", "").Replace("+", "");
               
                rabbitMqBusFactoryConfigurator.ReceiveEndpoint(endpointName,
                ec =>
                {
                    ec.ConfigureExecuteActivity(busRegistrationContext, typeof(CommandActivity<>).MakeGenericType(commandType));
                });
            }
        }
    }
}