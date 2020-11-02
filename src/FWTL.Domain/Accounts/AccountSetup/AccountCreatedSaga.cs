using System;
using Automatonymous;
using FWTL.Events;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AccountSetupState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid AccountId { get; set; }
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
                .Then(x => Console.WriteLine(x.Data.UserId))
                .Activity(x => x.OfType<SagaActivity<AccountSetupState,AddAccount.Command>>())
                .Publish(x => new CreateSession.Command()
                {
                    CorrelationId = x.CorrelationId.Value,
                    AccountId = x.Instance.AccountId
                })
                .TransitionTo(Initialized));

            During(Initialized, When(SessionCreatedEvent)
                .Then(x => Console.WriteLine(x.Data.AccountId))
                //.Activity(x => x.OfType<AddAccount.Handler>())
                //.Send(x => new AddAccount.Command() { UserId = x.Data.AccountId })
                .TransitionTo(WithSession));

            During(WithSession, When(CodeSentEvent).TransitionTo(WaitForCode));
            During(WaitForCode, When(CodeAcceptedEvent).TransitionTo(Ready));
        }

        public Event<AddAccount.Command> AddAccountCommand { get; }

        public Event<CreateSession.Command> SessionCreatedEvent { get; }

        public Event<CodeSent> CodeSentEvent { get; }

        public Event<CodeAccepted> CodeAcceptedEvent { get; }
    }
}