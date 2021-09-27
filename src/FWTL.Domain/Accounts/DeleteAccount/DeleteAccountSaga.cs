using Automatonymous;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Domain.Events;

namespace FWTL.Domain.Accounts.DeleteAccount
{
    //https://miro.com/app/board/o9J_lDZlCU8=/
    public class DeleteAccountSaga : MassTransitStateMachine<DeleteAccountState>
    {
        public State Delete { get; }

        public DeleteAccountSaga()
        {
            Event(() => SessionRemoved, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountDeleted, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SessionUnlinked, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => SessionNotFound, x => x.CorrelateById(m => m.Message.AccountId));

            InstanceState(x => x.CurrentState, Delete);

            Initially(When(AccountDeleted)
                .TransitionTo(Delete)
                .Publish(x => new RemoveSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            DuringAny(When(SessionRemoved)
                .TransitionTo(Delete)
                .Publish(x => new UnlinkSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(Delete, When(SessionUnlinked).Finalize());
            During(Delete, When(SessionNotFound).Finalize());

            SetCompletedWhenFinalized();
        }

        public Event<SessionRemoved> SessionRemoved { get; }

        public Event<AccountDeleted> AccountDeleted { get; }

        public Event<SessionUnlinked> SessionUnlinked { get; }

        public Event<SessionNotFound> SessionNotFound { get; }
    }
}