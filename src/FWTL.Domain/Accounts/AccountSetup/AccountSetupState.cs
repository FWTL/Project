using System;
using Automatonymous;
using MassTransit;
using MassTransit.Topology.Topologies;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AccountSetupState : SagaStateMachineInstance
    {
        public Guid AccountId { get; set; }
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public string ResponseAddress { get; set; }
        public Guid? RequestId { get; set; }
    }
}