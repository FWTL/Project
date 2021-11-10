using System;
using FWTL.Core.Events;
using FWTL.Domain.Accounts;

namespace FWTL.Domain.Events
{
    public class AccountSetupRestarted : IEvent
    {
        public Guid AccountId { get; set; }
        public Guid CorrelationId { get; set; }
        public AccountAggregate.AccountState State { get; set; }
    }
}