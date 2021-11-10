using System;
using Automatonymous;
using FWTL.Core.Events;
using FWTL.Domain.Events;
using Microsoft.Extensions.Logging;

namespace FWTL.Domain.Accounts.AccountSetup
{
    //https://miro.com/app/board/o9J_lDZlCU8=/
    public class AccountSetupSaga : MassTransitStateMachine<AccountSetupState>
    {
        public State InfrastructureSetup { get; }

        public State TelegramSetup { get; }

        public State Reset { get; }

        public AccountSetupSaga(ILogger<AccountSetupSaga> logger)
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
            Event(() => InfrastructureGenerated, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => CodeSent, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountVerified, x => x.CorrelateById(m => m.Message.AccountId));

            Event(() => SetupFailed, x => x.CorrelateById(m => m.Message.AccountId));

            Event(() => AccountSetupRestarted, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => AccountDeleted, x => x.CorrelateById(m => m.Message.AccountId));
            Event(() => TearDownInfrastructureCompleted, x => x.CorrelateById(m => m.Message.CorrelationId));

            InstanceState(x => x.CurrentState, InfrastructureSetup, TelegramSetup);

            Schedule(() => Timeout, instance => instance.ExpirationTokenId, s =>
            {
                s.Delay = TimeSpan.FromMinutes(5);
            });
            DuringAny(When(Timeout.Received).Publish(x => x.Data));

            Initially(When(AccountCreated)
                .TransitionTo(InfrastructureSetup)
                .Publish(x => new GenerateInfrastructure.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));


            Initially(When(AccountSetupRestarted)
                .TransitionTo(Reset)
                .Publish(x => new TearDownInfrastructure.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            Initially(When(AccountSetupRestarted)
                .TransitionTo(Reset)
                .Publish(x => new TearDownInfrastructure.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            //Initially(When(AccountSetupRestarted)
            //    .IfElse(x => x.Data.State >= AccountAggregate.AccountState.WithInfrastructure,
            //        x => 
            //            x.TransitionTo(Reset)
            //            .Publish(context => new TearDownInfrastructure.Command { CorrelationId = context.Data.CorrelationId, AccountId = context.Instance.CorrelationId }),
            //        x => x.TransitionTo()

            //.if(x => x.Data.State >= AccountAggregate.AccountState.WithInfrastructure
            //{
            //    var @event = x.Data;
            //    if (@event.State >= AccountAggregate.AccountState.WithInfrastructure)
            //    {
            //        x.Publish(new TearDownInfrastructure.Command { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId });
            //    }
            //    else
            //    {
            //        x.Publish(new GenerateInfrastructure.Command { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId });
            //    }
            //})

            During(Reset, When(TearDownInfrastructureCompleted)
                .TransitionTo(InfrastructureSetup)
                .Publish(x => new GenerateInfrastructure.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(InfrastructureSetup, When(InfrastructureGenerated)
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
                .TransitionTo(InfrastructureSetup)
                .Publish(x => new TearDownInfrastructure.Command() { CorrelationId = x.Data.CorrelationId, AccountId = x.Instance.CorrelationId }));

            During(TelegramSetup, When(InfrastructureTearedDown).Finalize());
            During(InfrastructureSetup, When(SetupFailed).Finalize());

            DuringAny(When(AccountDeleted).Finalize());

            Finally(binder => binder.Then(context => logger.LogInformation($"Saga {context.Instance.CorrelationId} finalized")));

            SetCompletedWhenFinalized();
        }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<InfrastructureGenerated> InfrastructureGenerated { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<CodeSent> CodeSent { get; }

        public Event<AccountVerified> AccountVerified { get; }

        public Event<SetupFailed> SetupFailed { get; }

        public Event<AccountSetupRestarted> AccountSetupRestarted { get; }

        public Event<AccountDeleted> AccountDeleted { get; }

        public Event<InfrastructureTearedDown> InfrastructureTearedDown { get; }

        public Event<CommandCompleted<TearDownInfrastructure.Command>> TearDownInfrastructureCompleted { get; }

        public Schedule<AccountSetupState, TearDownInfrastructure.Command> Timeout { get; }
    }
}