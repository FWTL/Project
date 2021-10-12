using System;
using Automatonymous;
using MassTransit.Saga;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AccountSetupState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public int CurrentState { get; set; }

        public int Version { get; set; }

        public Guid? ExpirationTokenId { get; set; }
    }
}