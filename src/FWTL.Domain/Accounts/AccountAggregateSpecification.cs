using System;
using System.Linq;
using FluentValidation;
using FWTL.Common.Helpers;
using FWTL.Common.Validators;
using FWTL.Core.Specification;
using FWTL.Database.Access;
using FWTL.Domain.Accounts.AccountSetup;
using FWTL.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Domain.Accounts
{
    public class AccountAggregateSpecification : AppAbstractValidation<AccountAggregate>,
        ISpecificationForEvent<AccountAggregate, AccountCreated>,
        ISpecificationForEvent<AccountAggregate, AccountDeleted>,
        ISpecificationForCommand<AccountAggregate, GenerateInfrastructure.Command>,
        ISpecificationForCommand<AccountAggregate, CreateSession.Command>,
    ISpecificationForCommand<AccountAggregate, Reset.Command>,
        ISpecificationForCommand<AccountAggregate, SendCode.Command>,
        ISpecificationForCommand<AccountAggregate, TearDownInfrastructure.Command>,
        ISpecificationForCommand<AccountAggregate, VerifyAccount.Command>
    {
        private readonly IDatabaseContext _dbContext;

        public AccountAggregateSpecification(IDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ValidateExternalAccountId()
        {
            RuleFor(x => x.ExternalAccountId).NotEmpty().Matches(RegexExpressions.OnlyNumbers).MaximumLength(20);
        }

        public void MustBeUnique()
        {
            RuleFor(x => x).CustomAsync(async (aggregate, context, token) =>
            {
                bool doesExist = await _dbContext.Accounts.AsQueryable()
                    .Where(account => account.OwnerId == aggregate.OwnerId)
                    .Where(account => account.ExternalAccountId == aggregate.ExternalAccountId)
                    .Where(account => account.Id != aggregate.Id).AnyAsync(token);

                if (doesExist)
                {
                    context.AddFailure(nameof(AccountAggregate), "Account already exists");
                }
            });
        }

        public void MustBeInState(AccountAggregate.AccountState state)
        {
            RuleFor(x => x.State).Equal(state);
        }

        public void MustHaveInfrastructure()
        {
            RuleFor(x => x).Must(x => x.HasInfrastructure());
        }

        public void MustBeOwner(Guid ownerId)
        {
            RuleFor(x => x.OwnerId).Equal(ownerId);
        }

        public IValidator<AccountAggregate> Apply(AccountCreated @event)
        {
            ValidateExternalAccountId();
            MustBeUnique();
            return this;
        }

        public IValidator<AccountAggregate> Apply(AccountDeleted @event)
        {
            MustBeOwner(@event.DeletedBy);
            return this;
        }

        public IValidator<AccountAggregate> Apply(GenerateInfrastructure.Command command)
        {
            MustBeInState(AccountAggregate.AccountState.Ready);
            return this;
        }

        public IValidator<AccountAggregate> Apply(CreateSession.Command command)
        {
            MustBeInState(AccountAggregate.AccountState.WithInfrastructure);
            return this;
        }

        public IValidator<AccountAggregate> Apply(Reset.Command command)
        {
            MustHaveInfrastructure();
            return this;
        }

        public IValidator<AccountAggregate> Apply(SendCode.Command command)
        {
            MustBeInState(AccountAggregate.AccountState.WithSession);
            return this;
        }

        public IValidator<AccountAggregate> Apply(TearDownInfrastructure.Command command)
        {
            MustHaveInfrastructure();
            return this;
        }

        public IValidator<AccountAggregate> Apply(VerifyAccount.Command command)
        {
            MustBeInState(AccountAggregate.AccountState.WaitForCode);
            return this;
        }
    }
}