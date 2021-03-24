using System;
using System.Collections.Generic;
using System.Text;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class AccountDeleted : IEvent
    {
        public Guid DeletedBy { get; set; }
        public Guid AccountId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
