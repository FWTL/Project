using System;
using Automatonymous;
using MassTransit.Saga;

namespace FWTL.Domain.Accounts.RestartSetup
{
    public class RestartAccountSetupState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public int CurrentState { get; set; }

        public int Version { get; set; }

        public Guid? ExpirationTokenId { get; set; }
    }
}