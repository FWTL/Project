using System;

namespace FWTL.Core.Events
{
    public class UnhandledExceptionOccured : IEvent
    {
        public string CommandFullName { get; set; }
        public Guid CommandCorrelationId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}