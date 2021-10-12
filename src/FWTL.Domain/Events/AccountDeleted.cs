using System;
using FWTL.Core.Events;

namespace FWTL.Domain.Events
{
    public class AccountDeleted : IEvent
    {
        public Guid DeletedBy { get; set; }

        public Guid AccountId { get; set; }

        public int State { get; set; }

        public Guid CorrelationId { get; set; }
    }
}