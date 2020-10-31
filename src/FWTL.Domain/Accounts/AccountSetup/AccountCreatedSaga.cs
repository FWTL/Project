using System;
using Automatonymous;
using FWTL.Domain.Accounts;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Events;

namespace FWTL.EventHandlers
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
            Event(() => AddAccountCommand, x => x.CorrelateById(context => context.Message.UserId));
            Event(() => SessionCreatedEvent, x => x.CorrelateById(context => context.Message.UserId));

            InstanceState(x => x.CurrentState, Initialized, WithSession, WaitForCode, Ready);

            Initially(When(AddAccountCommand)
                .Then(x => Console.WriteLine(x.Data.UserId))
                .Activity(x => x.OfType<AddAccount.Handler>())
                .Publish(x => new CreateSession.Command() { UserId = x.Data.UserId })
                .TransitionTo(Initialized));

            During(Initialized, When(SessionCreatedEvent)
                .Then(x => Console.WriteLine(x.Data.UserId))
                //.Activity(x => x.OfType<AddAccount.Handler>())
                .Send(x => new AddAccount.Command() { UserId = x.Data.UserId })
                .TransitionTo(Initialized));

            During(WithSession, When(CodeSentEvent).TransitionTo(WaitForCode));
            During(WaitForCode, When(CodeAcceptedEvent).TransitionTo(Ready));
        }

        public Event<AddAccount.Command> AddAccountCommand { get; }

        public Event<CreateSession.Command> SessionCreatedEvent { get; }

        public Event<CodeSent> CodeSentEvent { get; }

        public Event<CodeAccepted> CodeAcceptedEvent { get; }
    }
}