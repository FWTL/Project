using System;
using System.Collections.Generic;
using System.Text;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class CodeAccepted : IEvent
    {
        public Guid CorrelationId { get; }
    }
}
