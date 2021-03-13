using System;
using Automatonymous;
using FWTL.Core.Commands;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Topology.Topologies;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AccountSetupState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int Version { get; set; }
    }
}