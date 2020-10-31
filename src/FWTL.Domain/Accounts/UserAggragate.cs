using FWTL.Common.Aggregates;
using System;
using System.Collections.Generic;
using FWTL.Aggregate;
using FWTL.Core.Aggregates;
using FWTL.Events;

namespace FWTL.Domain.Accounts
{
    public class UserAggregate : AggregateRoot<UserAggregate>,
        IApply<AccountCreated>
    {
        public List<Account> Accounts { get; } = new List<Account>();

        public void AddAccount(Guid accountId, AddAccount.Command command)
        {
            var locationCreated = new AccountCreated()
            {
                AccountId = accountId,
                ExternalAccountId = command.ExternalAccountId
            };

            AddEvent(locationCreated);
        }

        public void Apply(AccountCreated @event)
        {
            Accounts.Add(new Account()
            {
                ExternalId = @event.ExternalAccountId,
                Id = @event.AccountId,
            });
        }
    }
}