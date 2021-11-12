using System;
using System.Threading.Tasks;
using Automatonymous;
using FWTL.Core.Aggregates;
using FWTL.Domain.Events;
using GreenPipes;

namespace FWTL.Domain.Accounts.AccountSetup.Activities
{
    public class AccountRestartedStrategy : Activity<AccountSetupState, AccountSetupRestarted>
    {
        private readonly IAggregateStore _aggregateStore;

        public AccountRestartedStrategy(IAggregateStore aggregateStore)
        {
            _aggregateStore = aggregateStore;
        }

        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<AccountSetupState, AccountSetupRestarted> context, Behavior<AccountSetupState, AccountSetupRestarted> next)
        {
            var account = await _aggregateStore.GetByIdAsync<AccountAggregate>(context.Data.AccountId);
            if (account.HasInfrastructure())
            {
                await context.Publish(new TearDownInfrastructure.Command() { AccountId = context.Data.AccountId, CorrelationId = context.Instance.CorrelationId });
            }
            else
            {
                await context.Publish(new GenerateInfrastructure.Command() { AccountId = context.Data.AccountId, CorrelationId = context.Instance.CorrelationId });
            }
        }

        public Task Faulted<TException>(BehaviorExceptionContext<AccountSetupState, AccountSetupRestarted, TException> context, Behavior<AccountSetupState, AccountSetupRestarted> next) where TException : Exception
        {
            throw new NotImplementedException();
        }
    }
}