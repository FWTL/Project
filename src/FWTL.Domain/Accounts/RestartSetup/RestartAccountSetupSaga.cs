using System;
using Automatonymous;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Events;

namespace FWTL.Domain.Accounts.RestartSetup
{
    //https://miro.com/app/board/o9J_lDZlCU8=/
    public class RestartAccountSetupSaga : MassTransitStateMachine<RestartAccountSetupState>
    {
        public State Restart { get; }

        public RestartAccountSetupSaga()
        {
            Event(() => SessionRemoved, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountDeleted, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SessionUnlinked, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SessionNotFound, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SessionCreated, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => CodeSent, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountVerified, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SetupFailed, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountSetupRestarted, x => x.CorrelateById(m => m.Message.AccountId));

            Schedule(() => Timeout, instance => instance.ExpirationTokenId, s =>
            {
                s.Delay = TimeSpan.FromMinutes(10);
            });

            InstanceState(x => x.CurrentState, Restart);

            Initially(When(AccountSetupRestarted)
                .TransitionTo(Restart)
                .Publish(x => new RemoveSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Restart, When(SessionRemoved)
                .Publish(x => new UnlinkSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Restart, When(SessionNotFound)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Restart, When(SessionUnlinked)
                .TransitionTo(Restart)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Restart, When(SessionCreated)
                .Publish(x => new SendCode.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Restart, When(CodeSent)
                .Schedule(Timeout, context => new RemoveSession.Command() { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId }));

            During(Restart, When(AccountVerified)
                .Unschedule(Timeout).Finalize());

            During(Restart, When(SetupFailed)
                .Unschedule(Timeout)
                .Publish(x => new RemoveSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId })
                .Finalize());

            DuringAny(When(AccountDeleted).Finalize());

            SetCompletedWhenFinalized();
        }

        public Event<AccountSetupRestarted> AccountSetupRestarted { get; }

        public Event<SessionRemoved> SessionRemoved { get; }

        public Event<SessionNotFound> SessionNotFound { get; }

        public Event<SessionUnlinked> SessionUnlinked { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<CodeSent> CodeSent { get; }

        public Event<AccountVeryfied> AccountVerified { get; }

        public Event<SetupFailed> SetupFailed { get; }

        public Event<AccountDeleted> AccountDeleted { get; }

        public Schedule<RestartAccountSetupState, RemoveSession.Command> Timeout { get; }
    }
}