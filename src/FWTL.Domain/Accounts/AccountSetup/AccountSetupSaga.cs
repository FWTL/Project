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

            Event(() => AccountSetupRestarted, x =>
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
            Event(() => AccountDeleted, x => x.CorrelateById(m => m.Message.AccountId));

            InstanceState(x => x.CurrentState, Initialized,  Ready);

            Schedule(() => Timeout, instance => instance.ExpirationTokenId, s =>
            {
                s.Delay = TimeSpan.FromMinutes(10);
            });

            WhenAccountIsCreatedCreateSession();
            WhenSessionIsCreatedSendCode();
            WhenCodeIsSentWaitForUserInputFor10MinBeforeRemovingSession();
            WhenAccountIsVerifiedFinalize();

            WhenSetupFailsScheduleRemoveSession();
            WhenAccountSetupRestarted();

            
            DuringAny(When(SessionRemoved)
                .TransitionTo(Initialized));

            DuringAny(When(AccountDeleted).Finalize());
            DuringAny(When(Timeout.Received).Publish(x => x.Data));
        }

        private void WhenAccountSetupRestarted()
        {
            Initially(When(AccountSetupRestarted)
                .Unschedule(Timeout)
                .TransitionTo(Initialized)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            DuringAny(When(AccountSetupRestarted)
                .Unschedule(Timeout));

            During(Initialized, When(AccountSetupRestarted)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(WithSession, When(AccountSetupRestarted)
                .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.CorrelationId }));

            During(WaitForCode, When(AccountSetupRestarted)
                .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.CorrelationId }));

        }

        public void WhenAccountIsCreatedCreateSession()
        {
            Initially(When(AccountCreated)
                .TransitionTo(Initialized)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

        }

        public void WhenSessionIsCreatedSendCode()
        {
            During(Initialized, When(SessionCreated)
                .TransitionTo(WithSession)
                .Publish(x => new SendCode.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

        }

        public void WhenCodeIsSentWaitForUserInputFor10MinBeforeRemovingSession()
        {
            During(WithSession, When(CodeSent)
                .Schedule(Timeout, context => new RemoveSession.Command() { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId })
                .TransitionTo(WaitForCode));
        }

        public void WhenAccountIsVerifiedFinalize()
        {
            During(WaitForCode, When(AccountVerified)
                .Unschedule(Timeout)
                .TransitionTo(Ready).Finalize());
        }

        public void WhenSetupFailsScheduleRemoveSession()
        {
            DuringAny(When(SetupFailed)
                .Schedule(Timeout, context => new RemoveSession.Command() { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId }));
        }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<SessionRemoved> SessionRemoved { get; }

        public Event<CodeSent> CodeSent { get; }

        public Event<AccountVeryfied> AccountVerified { get; }

        public Event<SetupFailed> SetupFailed { get; }

        public Event<AccountSetupRestarted> AccountSetupRestarted { get; }

        public Event<AccountDeleted> AccountDeleted { get; }

        public Schedule<AccountSetupState, RemoveSession.Command> Timeout { get; }
    }
}