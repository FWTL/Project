using FWTL.Core.Events;
using System;

namespace FWTL.Events
{
    public class AccountCreated : IEvent
    {
        public Guid AccountId { get; set; }
        public Guid CorrelationId { get; set; }
        public string ExternalAccountId { get; set; }
        public Guid OwnerId { get; set; }
    }
}