using System;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class CodeSent : IEvent
    {
        public Guid CorrelationId { get; set; }
    }
}