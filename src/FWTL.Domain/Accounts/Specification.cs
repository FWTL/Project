using System;
using FluentValidation;
using FWTL.Core.Aggregates;
using FWTL.Core.Specification;
using FWTL.Core.Validation;
using FWTL.Events;

namespace FWTL.Domain.Accounts
{
    public class AccountAggregateSpecification : AppAbstractValidation<AccountAggregate>,
        ISpecificationFor<AccountAggregate, AccountCreated>
    {
        private IAggregateStore _aggregateStore;

        public AccountAggregateSpecification(IAggregateStore aggregateStore)
        {
            _aggregateStore = aggregateStore;
        }

        public void MustBeUnique()
        {
            RuleFor(x => x).CustomAsync(async (aggregate, context, token) =>
            {
                bool doesExist = await _aggregateStore.ExistsAsync<AccountAggregate>(aggregate.Id);
                if (doesExist)
                {
                    context.AddFailure(nameof(AccountAggregate), "Account already exists");
                }
            });
        }

        public IValidator<AccountAggregate> Apply(AccountCreated @event)
        {
            MustBeUnique();
            return this;
        }
    }
}