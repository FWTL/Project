using System;
using Automatonymous;
using FWTL.Core.Commands;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Topology.Topologies;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AccountSetupState : SagaStateMachineInstance, ISagaState, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public string ResponseAddress { get; set; }
        public Guid RequestId { get; set; }
        public int Version { get; set; }
    }
}