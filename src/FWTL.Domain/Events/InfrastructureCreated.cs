using System;
using FWTL.Core.Events;

namespace FWTL.Domain.Events
{
    public class InfrastructureGenerated : IEvent
    {
        public Guid AccountId { get; internal set; }
        public Guid CorrelationId { get; set; }
    }
}