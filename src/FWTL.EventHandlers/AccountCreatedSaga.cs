using System;
using System.Threading.Tasks;
using Automatonymous;
using FWTL.Core.Commands;
using FWTL.Domain.Accounts;
using FWTL.Events;
using MassTransit.Courier;

namespace FWTL.EventHandlers
{
    public class AccountSetupState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
    }

    public class ValidateImageActivity :
        IExecuteActivity<AccountCreated>
    {
        private ICommandDispatcher _commandDispatcher;

        public ValidateImageActivity(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<AccountCreated> context)
        {
            _commandDispatcher.DispatchAsync<CreateSession.COmmand>()
        }
    }

    public class AccountSetupSaga : MassTransitStateMachine<AccountSetupState>
    {
        public State Initialized { get; }

        public State WithSession { get; }

        public State WaitForCode { get; }

        public State Ready { get; }

        public AccountSetupSaga()
        {
            Event(() => AccountCreatedEvent, x => x.CorrelateById(context => context.Message.AccountId));
            Event(() => SessionCreatedEvent, x => x.CorrelateById(context => context.Message.AccountId));

            InstanceState(x => x.CurrentState, Initialized, WithSession, WaitForCode, Ready);

            Initially(When(AccountCreatedEvent)
                .Then(x => Console.WriteLine(x.Data.AccountId))
                .Activity()
                .TransitionTo(Initialized));

            During(Initialized, When(SessionCreatedEvent)
                .Then(x => Console.WriteLine(x.Data.CorrelationId))
                .TransitionTo(WithSession));

            During(WithSession, When(CodeSentEvent).TransitionTo(WaitForCode));
            During(WaitForCode, When(CodeAcceptedEvent).TransitionTo(Ready));
        }

        public Event<AccountCreated> AccountCreatedEvent { get; }

        public Event<SessionCreated> SessionCreatedEvent { get; }

        public Event<CodeSent> CodeSentEvent { get; }

        public Event<CodeAccepted> CodeAcceptedEvent { get; }
    }
}