using System;
using System.Collections.Generic;
using FWTL.Core.Events;
using FWTL.Domain.Accounts;

namespace FWTL.Domain.Events
{
    public class SetupFailed : IEvent
    {
        public Guid AccountId { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public Guid CorrelationId { get; set; }

        public AccountAggregate.AccountState CurrentState { get; set; }
    }
}