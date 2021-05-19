using System;
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
            Event(() => AccountCreated, x =>
            {
                x.CorrelateById(m => m.Message.AccountId);
                x.SetSagaFactory(context => new AccountSetupState
                {
                    CorrelationId = context.Message.AccountId,
                    ExpirationTokenId = Guid.NewGuid(),
                });

            });
            Event(() => SessionCreated, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => CodeSent, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SetupFailed, x => x.CorrelateById(m => m.Message.AccountId));

            InstanceState(x => x.CurrentState, Initialized, WithSession, WaitForCode, Ready);

            Schedule(() => Timout, instance => instance.ExpirationTokenId, s =>
            {
                s.Delay = TimeSpan.FromSeconds(60);
            });

            Initially(When(AccountCreated)
                .TransitionTo(Initialized)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Initialized, When(SessionCreated)
                 .TransitionTo(WithSession)
                 .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.CorrelationId }));

            During(Initialized, When(Timout.Received)
                 .Publish(x => x.Data));

            During(WithSession, When(CodeSent)
                .TransitionTo(WaitForCode));

            During(WaitForCode, When(AccountVerified)
                .TransitionTo(Ready).Finalize());

            //During(Initialized, When(SetupFailed)
            //    .Schedule(Timout, context => new CreateSession.Command() { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId }));
        }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<CodeSent> CodeSent { get; }

        public Event<AccountVeryfied> AccountVerified { get; }

        public Event<SetupFailed> SetupFailed { get; }

        public Schedule<AccountSetupState, CreateSession.Command> Timout { get; }
    }
}