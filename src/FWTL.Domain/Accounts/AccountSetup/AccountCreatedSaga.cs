using System;
using Automatonymous;
using FWTL.Events;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AccountSetupState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
    }

    public class AccountSetupSaga : MassTransitStateMachine<AccountSetupState>
    {
        public State Initialized { get; }

        public State WithSession { get; }

        public State WaitForCode { get; }

        public State Ready { get; }

        public AccountSetupSaga()
        {
            InstanceState(x => x.CurrentState, Initialized, WithSession, WaitForCode, Ready);

            Initially(When(AddAccountCommand)
                .Activity(x => x.OfType<SagaActivity<AccountSetupState, AddAccount.Command>>())
                .TransitionTo(Initialized));

            During(Initialized, When(AccountCreated).Publish(x => new CreateSession.Command() { CorrelationId = x.CorrelationId.Value, AccountId = new AccountAggregate(x.Data.OwnerId, x.Data.ExternalAccountId).Id }));
            During(Initialized, When(CreateSession)
                .Activity(x => x.OfType<SagaActivity<AccountSetupState, CreateSession.Command>>())
                .TransitionTo(WithSession));

            During(WithSession, When(SessionCreated).Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Data.AccountId }));
            During(WithSession, When(SendCode)
               .Activity(x => x.OfType<SagaActivity<AccountSetupState, SendCode.Command>>())
               .TransitionTo(WithSession));

            //.Activity(x => x.OfType<AddAccount.Handler>())
            //.Send(x => new AddAccount.Command() { UserId = x.Data.AccountId })
            //.TransitionTo(WithSession));
        }

        public Event<AddAccount.Command> AddAccountCommand { get; }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<CreateSession.Command> CreateSession { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<SendCode.Command> SendCode { get; }
    }
}