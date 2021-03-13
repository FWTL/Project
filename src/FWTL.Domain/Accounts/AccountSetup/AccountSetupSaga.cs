using Automatonymous;
using FWTL.Events;
using MassTransit;

namespace FWTL.Domain.Accounts.AccountSetup
{
    public class AccountSetupSaga : MassTransitStateMachine<AccountSetupState>
    {
        public State Initialized { get; }

        public State WithSession { get; }

        public State WaitForCode { get; }

        public State Ready { get; }

        public AccountSetupSaga()
        {
            Event(() => AccountCreated, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SessionCreated, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => CodeSent, x => x.CorrelateById(m => m.Message.AccountId));

            InstanceState(x => x.CurrentState, Initialized, WithSession, WaitForCode, Ready);

            Initially(When(AccountCreated)
                .TransitionTo(Initialized)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Initialized, When(SessionCreated)
                 .TransitionTo(WithSession)
                 .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.CorrelationId }));

            During(WithSession, When(CodeSent)
                .TransitionTo(WaitForCode));

            During(WaitForCode, When(AccountVeryfied)
                .TransitionTo(Ready).Finalize());
        }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<CodeSent> CodeSent { get; }

        public Event<AccountVeryfied> AccountVeryfied { get; }
    }
}