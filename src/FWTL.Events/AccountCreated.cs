using FWTL.Core.Events;
using System;

namespace FWTL.Events
{
    public class AccountCreated : IEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid AccountId { get; set; }
        public string ExternalAccountId { get; set; }
    }
}