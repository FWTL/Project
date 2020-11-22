using System;

namespace FWTL.Core.Events
{
    public interface IEvent
    {
        Guid CorrelationId { get; set; }
    }
}