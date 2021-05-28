using System;
using System.Collections.Generic;
using System.Text;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class SessionUnlinked : IEvent
    {
        public Guid AccountId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
