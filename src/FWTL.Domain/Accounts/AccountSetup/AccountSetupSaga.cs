using System;
using Automatonymous;
using FWTL.Domain.Accounts.DeleteAccount;
using FWTL.Domain.Events;

namespace FWTL.Domain.Accounts.AccountSetup
{
    //https://miro.com/app/board/o9J_lDZlCU8=/
    public class AccountSetupSaga : MassTransitStateMachine<AccountSetupState>
    {
        public State InfrastructureSetup { get; }

        public State TelegramSetup { get; }

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
            Event(() => InfrastructureCreated, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => CodeSent, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountVerified, x => x.CorrelateById(m => m.Message.AccountId));

            Event(() => SetupFailed, x => x.CorrelateById(m => m.Message.AccountId));
            
            Event(() => AccountSetupRestarted, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountDeleted, x => x.CorrelateById(m => m.Message.AccountId));

            InstanceState(x => x.CurrentState, InfrastructureSetup, TelegramSetup);

            Schedule(() => Timeout, instance => instance.ExpirationTokenId, s =>
            {
                s.Delay = TimeSpan.FromMinutes(10);
            });
            DuringAny(When(Timeout.Received).Publish(x => x.Data));

            Initially(When(AccountCreated)
                .TransitionTo(InfrastructureSetup)
                .Publish(x => new SetupInfrastructure.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(InfrastructureSetup, When(InfrastructureCreated)
                .TransitionTo(TelegramSetup)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(TelegramSetup, When(SessionCreated)
                .Publish(x => new SendCode.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(TelegramSetup, When(CodeSent)
                .Schedule(Timeout, context => new TearDownInfrastructure.Command() { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId }));

            During(TelegramSetup, When(AccountVerified)
                .Unschedule(Timeout).Finalize());

            During(TelegramSetup, When(SetupFailed)
                .Unschedule(Timeout)
                .Publish(x => new TearDownInfrastructure.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId })
                .Finalize());

            During(InfrastructureSetup, When(SetupFailed).Finalize());

            DuringAny(When(AccountSetupRestarted).Finalize());
            DuringAny(When(AccountDeleted).Finalize());

            
            SetCompletedWhenFinalized();
        }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<InfrastructureCreated> InfrastructureCreated { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<CodeSent> CodeSent { get; }

        public Event<AccountVerified> AccountVerified { get; }

        public Event<SetupFailed> SetupFailed { get; }

        public Event<AccountSetupRestarted> AccountSetupRestarted { get; }

        public Event<AccountDeleted> AccountDeleted { get; }

        public Schedule<AccountSetupState, TearDownInfrastructure.Command> Timeout { get; }
    }
}