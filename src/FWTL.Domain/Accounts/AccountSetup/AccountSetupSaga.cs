using System;
using Automatonymous;
using FWTL.Common.Commands;
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
            InstanceState(x => x.CurrentState, Initialized, WithSession, WaitForCode, Ready);

            Event(() => AddAccountCommand, x =>
            {
                x.SetSagaFactory(context => new AccountSetupState()
                {
                    ResponseAddress = context.ResponseAddress.ToString(),
                    CorrelationId = context.CorrelationId.Value,
                    RequestId = context.RequestId
                });
            });

            Initially(When(AddAccountCommand)
                .Activity(x => x.OfType<SagaActivity<AccountSetupState, AddAccount.Command>>())
                .ThenAsync(async c =>
                {
                    //Send response back to orignial requestor once we are done with this step               
                    ISendEndpoint responseEndpoint =
                        await c.GetSendEndpoint(new Uri(c.Instance.ResponseAddress));

                    await responseEndpoint.Send(new Response(Guid.NewGuid()), callback: sendContext => sendContext.RequestId = c.Instance.RequestId);
                })
                .TransitionTo(Initialized));

            During(Initialized, When(AccountCreated)
                .Then(x => x.Instance.AccountId = x.Data.AccountId)
                .Publish(x => new CreateSession.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Instance.AccountId }));
            During(Initialized, When(CreateSession)
                .Activity(x => x.OfType<SagaActivity<AccountSetupState, CreateSession.Command>>())
                .TransitionTo(WithSession));

            During(WithSession, When(SessionCreated)
                .Publish(x => new SendCode.Command() { CorrelationId = x.CorrelationId.Value, AccountId = x.Data.AccountId }));
            During(WithSession, When(SendCode)
                .Activity(x => x.OfType<SagaActivity<AccountSetupState, SendCode.Command>>())
                .TransitionTo(WithSession));

            //.Activity(x => x.OfType<AddAccount.Handler>())
            //.Send(x => new AddAccount.Command() { UserId = x.Data.AccountId })
            //.TransitionTo(WithSession));
        }

        public Event<AddAccount.Command> AddAccountCommand { get; }

        public Event<AccountCreated> AccountCreated { get; }

        public Event<CreateSession.Command> CreateSession { get; }

        public Event<SessionCreated> SessionCreated { get; }

        public Event<SendCode.Command> SendCode { get; }
    }
}