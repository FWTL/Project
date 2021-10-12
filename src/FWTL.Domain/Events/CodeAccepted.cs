using System;
using FWTL.Core.Events;

namespace FWTL.Domain.Events
{
    public class CodeAccepted : IEvent
    {
        public Guid CorrelationId { get; set; }
    }
}