using System;
using FWTL.Core.Events;
using FWTL.Domain.Accounts;

namespace FWTL.Domain.Events
{
    public class AccountDeleted : IEvent
    {
        public Guid DeletedBy { get; set; }

        public Guid AccountId { get; set; }

        public AccountAggregate.AccountState State { get; set; }

        public Guid CorrelationId { get; set; }
    }
}