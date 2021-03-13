using System;
using FWTL.Core.Events;

namespace FWTL.Events
{
    public class AccountVeryfied : IEvent
    {
        public Guid CorrelationId { get; set; }

        public Guid AccountId { get; set; }
    }
}