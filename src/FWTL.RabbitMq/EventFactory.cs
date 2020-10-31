using System;
using EventStore.Client;
using FWTL.Core.Events;
using MassTransit;
using System.Collections.Generic;

namespace FWTL.RabbitMq
{
    public class EventFactory : IEventFactory
    {
        private readonly ConsumeContext _context;

        public EventFactory(ConsumeContext context)
        {
            _context = context;
        }

        public IEnumerable<EventComposite> Make(IEnumerable<EventComposite> @events)
        {
            foreach (var @event in @events)
            {
                @event.Metadata.EventId = Uuid.NewUuid();
                //@event.Event.CorrelationId = Guid.NewGuid();
                //@event.Metadata.CorrelationId = commandComposite.Metadata.CorrelationId;
                //@event.Metadata.CommandId = commandComposite.Metadata.CommandId;
                @event.Metadata.EventType = @event.Event.GetType().AssemblyQualifiedName;
            }

            return @events;
        }
    }
}