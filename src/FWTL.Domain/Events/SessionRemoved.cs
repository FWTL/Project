using System;
using FWTL.Core.Events;

namespace FWTL.Domain.Events
{
    public class SessionRemoved : IEvent
    {
        public Guid AccountId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}