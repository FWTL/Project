using System;
using FWTL.Core.Events;

namespace FWTL.Domain.Events
{
    public class AggregateInOutOfSyncState : IEvent
    {
        public Guid CorrelationId { get; set; }

        public Guid AggregateId { get; set; }

        public string Type { get; set; }

        public long Version { get; set; }
    }
}