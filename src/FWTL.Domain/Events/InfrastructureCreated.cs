using System;
using System.Collections.Generic;
using System.Text;
using FWTL.Core.Events;

namespace FWTL.Domain.Events
{
    public class InfrastructureCreated : IEvent
    {
        public Guid AccountId { get; internal set; }
        public Guid CorrelationId { get; set; }
    }
}
