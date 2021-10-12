using System;
using FWTL.Core.Events;

namespace FWTL.Domain.Events
{
    public class AccountVerified : IEvent
    {
        public Guid CorrelationId { get; set; }

        public Guid AccountId { get; set; }
    }
}