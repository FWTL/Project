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
            Event(() => SessionRemoved, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountSetupRestarted, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountDeleted, x => x.CorrelateById(m => m.Message.AccountId));

            InstanceState(x => x.CurrentState, Initialized, WithSession, WaitForCode, Ready);

            Schedule(() => Timout, instance => instance.ExpirationTokenId, s =>
            {
                s.Delay = TimeSpan.FromMinutes(10);
            });

            //When account is created publish create session command
            Initially(When(AccountCreated)
                .TransitionTo(Initialized)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            Initially(When(AccountSetupRestarted)
                .TransitionTo(Initialized)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            //When session is created publish send code
            During(Initialized, When(SessionCreated)
                 .TransitionTo(WithSession)
                 .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.CorrelationId }));

            //When code is sent schedule timeout and wait for verification
            During(WithSession, When(CodeSent)
                .Schedule(Timout, context => new RemoveSession.Command() { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId })
                .TransitionTo(WaitForCode));

            //When account is verified, unscheduled timeout and mark as ready
            During(WaitForCode, When(AccountVerified)
                .Unschedule(Timout)
                .TransitionTo(Ready).Finalize());

            //When setup fails, schedule remove session command
            DuringAny(When(SetupFailed)
                .Schedule(Timout, context => new RemoveSession.Command() { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId }));

            DuringAny(When(Timout.Received).Publish(x => x.Data));

            DuringAny(When(SessionRemoved)
                .TransitionTo(Initialized));

            DuringAny(When(AccountSetupRestarted)
                .Unschedule(Timout));

            During(Initialized, When(AccountSetupRestarted)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(WithSession, When(AccountSetupRestarted)
                .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.CorrelationId }));

            During(WaitForCode, When(AccountSetupRestarted)
                .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.CorrelationId }));

            DuringAny(When(AccountDeleted).Finalize());
        }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<SessionRemoved> SessionRemoved { get; }

        public Event<CodeSent> CodeSent { get; }

        public Event<AccountVeryfied> AccountVerified { get; }

        public Event<SetupFailed> SetupFailed { get; }

        public Event<AccountSetupRestarted> AccountSetupRestarted { get; }

        public Event<AccountDeleted> AccountDeleted { get; }

        public Schedule<AccountSetupState, RemoveSession.Command> Timout { get; }
    }
}