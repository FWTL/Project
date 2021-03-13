using System;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class CodeAccepted : IEvent
    {
        public Guid CorrelationId { get; set; }
    }
}
