using System;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class SessionNotFound : IEvent
    {
        public Guid AccountId { get; set; }

        public Guid CorrelationId { get; set; }
    }
}