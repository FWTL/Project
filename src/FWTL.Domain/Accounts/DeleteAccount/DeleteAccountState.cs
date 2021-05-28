using System;
using Automatonymous;
using MassTransit.Saga;

namespace FWTL.Domain.Accounts.DeleteAccount
{
    public class DeleteAccountState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public int CurrentState { get; set; }

        public int Version { get; set; }
    }
}